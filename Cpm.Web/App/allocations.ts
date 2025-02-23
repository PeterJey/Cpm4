import * as Common from "./common";
import ModalDialog from "./modaldialog";

interface IProduct {
    type: string;
    perTray: number;
    perPunnet: number;
}

export default class Allocations {
    constructor(siteId: string, position: string, preferredUnit: string) {
        this.siteId = siteId;

        $("#previousButton").click(() => this.loadWeek(--this.position));
        $("#todayButton").click(() => this.loadWeek(NaN));
        $("#nextButton").click(() => this.loadWeek(++this.position));

        this.preferredUnit = preferredUnit || "kg";

        this.loadWeek(parseInt(position, 10));
    }

    private siteId: string;

    private preferredUnit: string;

    private position: number;

    loadWeek(week: number) {
        Common.loadContent(
            `/Allocations/Week?siteid=${this.siteId}&position=${isNaN(week) ? "" : week}`,
            $("#contentgrid"),
            () => {
                this.position = parseInt($("#as-loaded").data("week"), 10);

                $("tr.alloc-row-day td.alloc-product-slot")
                    .click((event) => this.editAllocation($(event.delegateTarget as any), false));

                $("tr.alloc-row-day td.alloc-availability-slot")
                    .click((event) => this.editAllocation($(event.delegateTarget as any), true));

                $("#weekly-link")
                    .prop("href", `/Fields/WeeklyDiary?siteId=${this.siteId}&position=${this.position}`);

                this.highlightWarnings();
            });
    }

    editAllocation($element: JQuery, canEditProduct: boolean) {
        let dialog = new ModalDialog();
        dialog.showElement(
            $("#edit-allocation-template"),
            () => {
                const selectedProduct = $("#product-select").val();

                const fieldId = $element.parents("tr").data("fieldid");

                const product = this.getProduct();

                if (!selectedProduct && !product) {
                    dialog.message = `Please select existing product or complete all 3 fields describing the product`;
                    return;
                }

                const weight = this.convertUnits(
                    this.convertToInt($("#amount-box").val(), 0),
                    $("#unit-box").val() as string,
                    "kg",
                    product.perTray,
                    product.perPunnet
                );

                Common.makePostRequest(
                    "/Allocations/Save",
                    {
                        fieldId: fieldId,
                        date: $("#date-box").val(),
                        product,
                        weight
                    },
                    (data) => {
                        this.loadWeek(this.position);
                        dialog.hide();
                    },
                    () => {
                        dialog.message = "Could not save the allocation";
                    });
            },
            () => {
                dialog.hide();
            },
            () => {
                $("#field-box").val($element.parents("tr").data("field"));
                $("#date-box").val($element.parents("tr").data("date"));
                $("#product-select").val($element.data("product"));

                $("#product-per-tray-box, #product-per-punnet-box, #amount-box")
                    .on("input", () => this.updateWeight());

                $("#unit-box")
                    .val(this.preferredUnit)
                    .on("change", () => this.updateWeight());

                if (canEditProduct) {
                    $("#product-select").removeAttr("disabled");
                    $("#product-select")
                        .val($element.parents(".alloc-product-slot").data("product"))
                        .on("change", () => this.updateProductBoxes());
                } else {
                    $("#product-select").attr("disabled", "disabled");
                }
                this.updateProductBoxes();

                const product = this.getProduct();
                const weight = this.convertToInt($element.data("weight"), 0);

                if (product) {
                    const amount = this.convertUnits(
                        weight,
                        "kg",
                        $("#unit-box").val() as string,
                        product.perTray,
                        product.perPunnet
                    );
                    $("#amount-box").val(amount ? Math.round(amount).toLocaleString() : "");
                } else {
                    $("#amount-box").val(weight);
                    $("#unit-box").val("kg");
                }
                this.updateWeight();
            });
    }

    getProduct(): IProduct {
        const type = $("#product-type-box").val() as string;
        const perTray = this.convertToInt($("#product-per-tray-box").val());
        const perPunnet = this.convertToInt($("#product-per-punnet-box").val());

        if (!perTray || !perPunnet) {
            return null;
        }

        const product: IProduct = {
            type: type,
            perTray,
            perPunnet,
        };

        return product;
    }

    convertUnits(amount: number, fromUnit: string, toUnit: string, perTray: number, perPunnet: number): number {
        let factor = 1;

        switch (fromUnit) {
            case "kg":
                break;
            case "trays":
                factor *= perPunnet / 1000 * perTray;
                break;
            case "punnets":
                factor *= perPunnet / 1000;
                break;
            default:
                factor = NaN;
        }

        switch (toUnit) {
            case "kg":
                break;
            case "trays":
                factor *= 1000 / perPunnet / perTray;
                break;
            case "punnets":
                factor *= 1000 / perPunnet;
                break;
            default:
                factor = NaN;
        }

        return amount * factor;
    }

    copyProductToEdits() {
        const parts = $("#product-select").val().toString().split("|");
        $("#product-type-box").val(parts[0]);
        $("#product-per-tray-box").val(parts[1]);
        $("#product-per-punnet-box").val(parts[2]);
    }

    updateWeight() {
        const perTray = this.convertToInt($("#product-per-tray-box").val());
        const perPunnet = this.convertToInt($("#product-per-punnet-box").val());

        const amount = this.convertToInt($("#amount-box").val(), 0);

        const unit = $("#unit-box").val() as string;

        const weight = this.convertUnits(
            amount,
            unit,
            "kg",
            perTray, perPunnet
        );

        $("#weight-box").val(weight.toLocaleString() + " kg");

        if (unit === "kg" || isNaN(weight)) {
            $("#weight-box").parent().hide();
        } else {
            $("#weight-box").parent().show();
        }
    }

    updateProductBoxes()
    {
        this.copyProductToEdits();
        if ($("#product-select").val()) {
            $("#product-type-box,#product-per-tray-box,#product-per-punnet-box")
                .parent()
                .hide();
        } else {
            $("#product-type-box,#product-per-tray-box,#product-per-punnet-box")
                .parent()
                .show();
        }
        this.updateWeight();
    }

    convertToInt(data: any, defaultValue: number = NaN): number {
        return parseInt((data || "").toString().split(",").join(""), 10) || defaultValue;
    }

    highlightWarnings() {
        const correctionFactorForOver = 1.1;
        const correctionFactorForUnder = 1.1;
        $("tr")
            .each((index, tr) => {
                const available = this.convertToInt(
                    $(tr as any)
                        .find(".alloc-availability-slot")
                        .data("weight"),
                    0
                );
                const allocated = this.convertToInt(
                    $(tr as any)
                    .find(".alloc-product-total")
                        .data("weight"),
                    0
                );

                if (allocated > available*correctionFactorForOver) {
                    $(tr as any)
                        .find("td:not(:first-of-type)")
                        .addClass("alloc-cell-overallocated");
                } else if (allocated < available/correctionFactorForUnder) {
                    $(tr as any)
                        .find("td:not(:first-of-type)")
                        .addClass("alloc-cell-underallocated");
                }
            });
    }
}

