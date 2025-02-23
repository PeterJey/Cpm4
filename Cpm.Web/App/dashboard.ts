import * as Common from "./common";
import ModalDialog from "./modaldialog";

export default class Dashboard {

    constructor(postcode: string, siteId: string) {
        this.postcode = postcode;
        this.siteId = siteId;

        this.loadFields();

        if (this.postcode) {
            this.loadWeatherReport();
            $("#reloadWeather").click(() => this.loadWeatherReport());
        }
    }

    loadWeatherReport() {
        Common.loadContent(
            `/Weather/Report?postcode=${this.postcode}`,
            $("#weatherReport"));
    }

    loadFields() {
        $("#scenarioFields").html(`<p>Loading fields...</p>`);

        $(".active-scenario-select").off("change");

        Common.loadContent(
            `/Sites/Summary?siteid=${this.siteId}`,
            $("#scenarioFields"),
            () => {
                $(".active-scenario-select")
                    .on("change", (event) => this.scenarioChanged(event))
                    .on("focus", (event) => this.scenarioFocused(event));

                $("button.change-field-description")
                    .on("click", (event) => this.changeFieldDescription(event));

                $("button.change-budget").click((event) => this.changeBudget(event));

                $("button.change-profile").click((event) => this.changeProfile(event));
            }
        );
    }

    changeBudget(event: JQuery.Event) {
        let dialog = new ModalDialog();
        let $row = $(event.delegateTarget as any).parents("tr");
        dialog.showElement(
            $("#edit-budget-template"),
            () => {
                let ypp = ($("#yield-ypp").val() as string).trim();
                let ppa = ($("#yield-ppa").val() as string).trim();
                let ypa = ($("#yield-ypa").val() as string).trim();

                if ((ypp && ppa && !ypa) || (!ypp && !ppa && ypa)) {
                    Common.makePostRequest(
                        "/Fields/Budget",
                        {
                            fieldid: $row.data("field-id"),
                            ypp, ppa, ypa 
                        },
                        () => {
                            dialog.hide();

                            this.loadFields();
                        },
                        () => {
                            dialog.message = "Could not change the budget.";
                        });

                } else {
                    dialog.message = "Please provide values only for one type of budget.";
                }
            },
            () => {
                dialog.hide();
            },
            () => {
                $("#yield-ypp").val($row.data("ypp"));
                $("#yield-ppa").val($row.data("ppa"));
                $("#yield-ypa").val($row.data("ypa"));
            });
    }

    changeProfile(event: JQuery.Event) {
        let dialog = new ModalDialog();
        let $row = $(event.delegateTarget as any).parents("tr");
        dialog.showElement(
            $("#change-profile-template"),
            () => {
                Common.makePostRequest(
                    "/Fields/Profile",
                    {
                        fieldid: $row.data("field-id"),
                        profilename: $("#profile-box").val()
                    },
                    () => {
                        dialog.hide();

                        this.loadFields();
                    },
                    () => {
                        dialog.message = "Could not change the budget.";
                    });
            },
            () => {
                dialog.hide();
            },
            () => {
                $("#profile-box").val($row.data("profile-name"));
            });
    }

    changeFieldDescription(event: JQuery.Event<Node, any>) {
        let $element = $(event.delegateTarget as any);
        let $descriptionElement = $element.siblings(".description-text");
        let dialog = new ModalDialog();
        dialog.showElement(
            $("#change-description-template"), 
            () => {
                dialog.isBusy = true;
                let description = $("#description-box").val() as string;
                Common.makePostRequest(
                    "/Fields/Description",
                    {
                        fieldid: $element.parents("tr").data("field-id"),
                        description,
                    },
                    () => {
                        dialog.hide();
                        $descriptionElement
                            .text(description);
                    },
                    () => {
                        dialog.isBusy = false;
                        dialog.message = "Could not change the description.";
                    }
                );
            }, 
            () => {
                dialog.hide(); 
            },
            () => {
                $("#description-box")
                    .val($descriptionElement.text())
                    .data("old-value", $("#description-box").val())
                    .on("input",
                        () => {
                            dialog.isValid = $("#description-box").val() !== $("#description-box").data("old-value");
                        })
                    .focus();
                dialog.isValid = false;
            },
            () => {
                $("#description-box")
                    .off("input");
            });
    }
    scenarioFocused(event: JQuery.Event<Node, any>) {
        let $element = $(event.delegateTarget as any);
        $element.data("oldValue", $element.val());
    }

    scenarioChanged(event: JQuery.Event<Node, any>) {
        let $element = $(event.delegateTarget as any);
        let oldValue = $element.data("old-value") as string;
        let fieldId = $element.data("field-id") as string;
        let newDescription = $element.find(`option[value="${$element.val()}"]`).text();
        let fieldName = $element.data("field-name") as string;

        let dialog = new ModalDialog();

        dialog.confirmPrimary(
            `Please confirm you want to change the scenario to "${newDescription}" for field "${fieldName}"`,
            "Changing active scenario",
            "Change", "Cancel",
            () => {
                Common.makePostRequest(
                    "/Fields/ActiveScenario",
                    { fieldid: fieldId, scenarioid: $element.val() },
                    () => {
                        dialog.hide();
                        this.loadFields();
                        Common.showNotice(`Field "${fieldName}" was updated.`);
                    },
                    () => {
                        dialog.hide();
                        $element.val(oldValue);
                        Common.showAlert(`Could not update the field "${fieldName}".`);
                    }
                );
            },
            () => {
                $element.val(oldValue);
                dialog.hide();
            }
        );
    }

    private siteId: string;
    private postcode: string;
}