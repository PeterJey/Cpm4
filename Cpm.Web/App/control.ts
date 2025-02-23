import * as Common from "./common";
import ModalDialog from "./modaldialog";
import * as Chart from "chart.js";

export default class Control {
    constructor(contextId: string, isDirty: string) {
        this.contextId = contextId;

        this.isDirty = (isDirty || "").toLowerCase() === "true";

        this.loadFields();
    }

    private chart: Chart;

    private fieldSelection: string;
    
    private oldAlgorithm: string;
    
    private _isDirty: boolean;

    get isDirty(): boolean {
        return this._isDirty;
    }
    set isDirty(value:boolean) {
        if (this._isDirty === value) {
            return;
        };
        this._isDirty = value;
        this.updateControls();
    }

    updateApplyState() {
        if (this.getFieldSelection() !== this.fieldSelection) {
            $("#apply-field-selection")
                .prop("disabled", false);
        } else {
            $("#apply-field-selection")
                .prop("disabled", true);
        }
    }

    getFieldSelection() {
        const values = $(".scenario-field-selection input")
            .toArray()
            .map(element => {
                return $(element as any).prop("checked") as boolean;
            });

        return values
            .reduce((aggregated, current) => {
                return aggregated += current ? "1" : "0";
            }, "");
    }

    startFieldSelection() {
        $("#cancel-field-selection,#apply-field-selection")
            .show();

        this.updateApplyState();

        $(".scenario-field-selection input")
            .on("change", () => this.updateApplyState());

        $(".scenario-field-selection")
            .show("fast");

        $("#show-field-selection,#save-scenario,#rename-scenario,#change-season-scores,#set-as-active,#duplicate-scenario")
            .hide();

        this.showAllRows();
    }

    resetFieldSelection() {
        this.fieldSelection = this.getFieldSelection();

        $("#field-selection-message,#cancel-field-selection,#apply-field-selection")
            .hide();

        $("#show-field-selection,#save-scenario,#rename-scenario,#change-season-scores,#set-as-active,#duplicate-scenario")
            .show();

        $(".scenario-field-selection input")
            .off("changed");

        $(".scenario-field-selection")
            .hide("fast");

        this.updateRowVisibility(false);
    }

    applyFieldSelection() {
        let selection = this.getFieldSelection();
        Common.makePostRequest(
            "/Scenarios/SelectedFields",
            {
                contextid: this.contextId, 
                selection
            },
            () => {
                this.fieldSelection = selection;
                this.isDirty = true;
                this.resetFieldSelection();
                this.loadResults();
            },
            () => {
                $("#field-selection-message")
                    .show();
            });
    }

    showAllRows(animate: boolean = true) {
        $(".scenario-field")
            .show("fast");
    }

    updateRowVisibility(animate: boolean = true) {
        $(".scenario-field")
            .each((index, element) => {
                let row = $((element) as any);
                if (row.find("input").prop("checked")) {
                    if (animate) {
                        row.show("fast");
                    } else {
                        row.show();
                    }
                } else {
                    if (animate) {
                        row.hide("fast");
                    } else {
                        row.hide();
                    }
                }
            });
    }

    loadFields() {
        this.loadResults();
        Common.loadContent(
            `/Scenarios/Fields?contextid=${this.contextId}`,
            $("#settings"),
            () => {
                $("#field-selection-message")
                    .hide();

                $(".scenario-field-selection")
                    .hide();

                $("#show-field-selection").click(() => this.startFieldSelection());
                $("#cancel-field-selection").click(() => this.resetFieldSelection());
                $("#apply-field-selection").click(() => this.applyFieldSelection());
                $("#save-scenario").click(() => this.saveScenario());
                $("#rename-scenario").click(() => this.renameScenario());
                $("#delete-scenario").click(() => this.deleteScenario());
                $("#change-season-scores").click(() => this.changeSeasonScores());
                $("#set-as-active").click(() => this.setAsActiveForFields());
                $("#duplicate-scenario").click(() => this.duplicateScenario());

                $(".change-week-offset").click((event) => this.changeWeekOffset(event));

                $(".suppress-dirty-check").off("click");
                $(".suppress-dirty-check").click(() => { this.isDirty = false; });

                this.updateWeekOffsetText();

                this.resetFieldSelection();

                this.updateControls();
            });
    }

    loadResults() {
        this.updateChart(false);
        Common.loadContent(
            `/Scenarios/ResultsGrid?contextid=${this.contextId}`,
            $("#results"),
            () => {
                $("#download-data-grid")
                    .click((event) => {
                        $(`<iframe id="download-iframe" style="display:none;" src="/Scenarios/DownloadGrid?contextid=${
                                this.contextId}"></iframe>`)
                            .appendTo("body");
                    });
                this.updateChart(true);
            });
    }

    updateChart(isValid: boolean) {
        if (!isValid) {

            return;
        }

        // r, g, b, a for line, a for fill
        const palette = [
            [255,  99, 132, 1, 0.2],
            [ 54, 162, 235, 1, 0.2],
            [255, 206,  86, 1, 0.2],
            [ 75, 192, 192, 1, 0.2],
            [153, 102, 255, 1, 0.2],
            [255, 159,  64, 1, 0.2]
        ];

        const borderColors = palette.map(v => `rgba(${v[0]}, ${v[1]}, ${v[2]}, ${v[3]})`);
        const backgroundColors = palette.map(v => `rgba(${v[0]}, ${v[1]}, ${v[2]}, ${v[4]})`);

        const datasets = $("#results .grid-data-series")
            .toArray()
            .map((x, index) => {
                return {
                    label: x.textContent,
                    data: [],
                    borderColor: borderColors[index % borderColors.length],
                    backgroundColor: backgroundColors[index % backgroundColors.length],
                    borderWidth: 1,
                    borderDash: [],
                    yAxisID: "series-axis"
                };
            });

        const labels = $("#results .grid-data-label")
            .toArray()
            .map(x => x.textContent);

        const extractNumber = (text: string) => (text || "").split(",").join("");

        const totals = $("#results .grid-data-total")
            .toArray()
            .map(x => extractNumber(x.textContent));

        const labour = $("#results .grid-data-labour")
            .toArray()
            .map(x => extractNumber(x.textContent));

        $("#results .grid-data-row")
            .each((ind, row) => {
                $(row as any)
                    .find(".grid-data-val")
                    .each((index, element) => {
                        (datasets[index] as any).data.push(extractNumber(element.textContent));
                    });
            });

        let tickLabelCallback = (value, index, values) => {
            return value.toLocaleString();
        };

        if (this.chart) {
            this.chart.destroy();
        }
        var backgroundColor = "white";
        Chart.pluginService.register({
            beforeDraw: (chart) => {
                var ctx = chart.ctx;
                ctx.fillStyle = backgroundColor;
                ctx.fillRect(0, 0, chart.canvas.width, chart.canvas.height);
            }
        });
        this.chart = new Chart("myChart", {
            type: "line",
            data: {
                labels,
                datasets:
                datasets
                    .concat([
                        {
                            label: "Total",
                            data: totals,
                            borderColor: "rgba(0,0,0,1)",
                            backgroundColor: "rgba(0,0,0,0.1)",
                            borderWidth: 1,
                            borderDash: [10],
                            yAxisID: "total-axis"
                        },
                        {
                            label: "Labour",
                            data: labour,
                            borderColor: "rgba(50,50,50,1)",
                            backgroundColor: "rgba(50,50,50,0.1)",
                            borderWidth: 1,
                            borderDash: [3],
                            yAxisID: "labour-axis"
                        },
                    ])
            },
            options: {
                scales: {
                    yAxes: [
                        {
                            id:
                                "series-axis",
                            scaleLabel: {
                                display: true,
                                labelString: "by field"
                            },
                            type: "linear",
                            position: "left",
                            ticks: {
                                callback: tickLabelCallback,
                            }
                        }, {
                            id: "total-axis",
                            scaleLabel: {
                                display: true,
                                labelString: "total"
                            },
                            type: "linear",
                            position: "right",
                            gridLines: {
                                borderDash: [10]
                            },
                            ticks: {
                                callback: tickLabelCallback
                            }
                        }, {
                            id: "labour-axis",
                            scaleLabel: {
                                display: true,
                                labelString: "labour"
                            },
                            type: "linear",
                            position: "right",
                            gridLines: {
                                borderDash: [3]
                            },
                            ticks: {
                                callback: tickLabelCallback,
                            }
                        }
                    ]
                },
                tooltips: {
                    mode: "x",
                    callbacks: {
                        label: (tooltipItem, data) => {
                            let label = data.datasets[tooltipItem.datasetIndex].label || "";

                            if (label) {
                                label += ": ";
                            }
                            label += parseFloat(tooltipItem.yLabel).toLocaleString();

                            return label;
                        },
                        beforeLabel: (tooltipItem, data) => {
                            return ["total-axis", "labour-axis"].indexOf(data.datasets[tooltipItem.datasetIndex].yAxisID) >= 0
                                ? " "
                                : "";
                        }
                    }
                },
                legend: {
                    onClick: (e) => e.stopPropagation()
                }
            }
        });
    }

    getSelectedOption(select: JQuery<HTMLElement>): JQuery<HTMLOptionElement> {
        let $select = $(select as any);
        return $select
            .find(`option[value="${$select.val()}"]`)
            .first() as JQuery<HTMLOptionElement>;
    }

    deleteScenario() {
        let dialog = new ModalDialog();
        dialog.confirmDanger(
            `Please confirm deleting the scenario.`,
            "Delete scenario",
            "Delete",
            "Keep",
            () => {
                Common.makePostRequest(
                    `/Scenarios/Delete`,
                    {
                        contextid: this.contextId,
                    },
                    () => {
                        // prevent "leaving page" warning
                        this.isDirty = false;
                        window.location
                            .replace(
                            $("#dashboard-link")
                                .attr("href")
                            );
                    },
                    () => {
                        dialog.message = "Could not delete the scenario.";
                    });
            },
            () => {
                dialog.hide();
            });
    }

    setAsActiveForFields() {
        let dialog = new ModalDialog();
        dialog.confirmPrimary(
            `Please confirm setting the scenario as active for all visible fields.`,
            "Set as active",
            "Set",
            "Cancel",
            () => {
                Common.makePostRequest(
                    `/Scenarios/ActiveScenarioForFields`,
                    {
                        contextid: this.contextId,
                    },
                    () => {
                        dialog.hide();
                        Common.showNotice("Active scenario was updated.");
                    },
                    () => {
                        dialog.message = "Could not set for all fields.";
                    });
            },
            () => {
                dialog.hide();
            });
    }

    duplicateScenario() {
        let dialog = new ModalDialog();
        dialog.confirmPrimary(
            `This will create new scenario with same settings as the current. Please confirm duplicating the scenario.`,
            "Duplicate scenario",
            "Duplicate",
            "Cancel",
            () => {
                Common.makePostRequest(
                    `/Scenarios/Duplicate`,
                    {
                        contextid: this.contextId,
                    },
                    (data) => {
                        window.location
                            .replace(`/Scenarios/Control?contextid=${data.contextid}`);
                    },
                    () => {
                        dialog.message = "Could not duplicate the scenario.";
                    });
            },
            () => {
                dialog.hide();
            });
    }

    renameScenario() {
        let dialog = new ModalDialog();

        dialog.showElement(
            $("#rename-scenario-template"), 
            () => {
                dialog.isBusy = true;
                let name = $("#name-box").val() as string;
                Common.makePostRequest(
                    "/Scenarios/Rename",
                    {
                        contextid: this.contextId,
                        name,
                    },
                    () => {
                        $("#original-name,#scenario-name")
                            .text(name);
                        this.isDirty = true;
                        dialog.hide();
                    },
                    () => {
                        dialog.isBusy = false;
                        dialog.message = "Could not rename the scenario.";
                    }
                );
            }, 
            () => {
                dialog.hide(); 
            },
            () => {
                $("#name-box")
                    .val($("#original-name").text())
                    .data("old-value", $("#name-box").val())
                    .on("input",
                        () => {
                            dialog.isValid = $("#name-box").val() !== $("#name-box").data("old-value");
                        })
                    .focus();
                dialog.isValid = false;
            },
            () => {
                $("#name-box")
                    .off("input");
            });
    }

    changeSeasonScores() {
        let dialog = new ModalDialog();

        dialog.showElement(
            $("#season-scores-template"), 
            () => {
                dialog.isBusy = true;
                let data = { contextid: this.contextId };
                $("#season-scores-template .season-score")
                    .each((index, element) => {
                        let $element = $(element as any);
                        data[$element.data("season")] = $element.val();
                    });
                Common.makePostRequest(
                    "/Scenarios/SeasonScores",
                    data,
                    () => {
                        // prevent "leaving page" warning
                        this.isDirty = false;
                        location.reload();
                        dialog.hide();
                    },
                    () => {
                        dialog.isBusy = false;
                        dialog.message = "Could not apply the changes.";
                    }
                );
            }, 
            () => {
                $("#season-scores-template .season-score")
                    .each((index, element) => {
                        let $element = $(element as any);
                        $element.val($element.data("old-value"));
                    });
                dialog.hide(); 
            },
            () => {
                $("#season-scores-template .season-score")
                    .each((index, element) => {
                        let $element = $(element as any);
                        $element.data("old-value", $element.val());
                    });
            });
    }

    changeWeekOffset(event: JQuery.Event<Node, any>) {
        let $button = $(event.delegateTarget as any);
        let $element = $button.siblings(".week-offset");
        let $field = $button.parents(".scenario-field");

        let fieldName = $field.data("field-name");
        let savedValue = parseInt($element.data("value"));

        let tryModifyOffset = (delta: number) => {
            let oldValue = parseInt($("#new-week-offset").data("value"));
            let newValue = oldValue + delta;

            $("#make-earlier").prop("disabled", newValue <= -5);
            $("#make-later").prop("disabled", newValue >= 5);

            // ReSharper disable once VariableUsedInInnerScopeBeforeDeclared
            dialog.isValid = newValue !== savedValue;

            if (newValue < -5 || newValue > 5) {
                return;
            }

            $("#new-week-offset")
                .data("value", newValue)
                .text(
                    this.describeWeekOffset(
                        parseInt(
                            $("#new-week-offset").data("value"),
                            10
                        )
                    )
                );
        }

        let dialog = new ModalDialog();

        dialog.showElement(
            $("#week-offset-template"),
            () => {
                dialog.isBusy = true;
                let newOffset = parseInt($("#new-week-offset").data("value") as string, 10);
                Common.makePostRequest(
                    "/Scenarios/WeekOffset",
                    {
                        contextid: this.contextId,
                        index: $field.data("field-ordinal"),
                        offset: newOffset
                    },
                    () => {
                        this.isDirty = true;
                        dialog.hide();
                        this.loadFields();
                    },
                    () => {
                        dialog.isBusy = false;
                        dialog.message = "Could not change the week offset.";
                    }
                );
            },
            () => {
                dialog.hide();
            },
            () => {
                $("#field-name")
                    .text(fieldName);
                $("#new-week-offset")
                    .data("value", savedValue);
                tryModifyOffset(0);
                $("#make-earlier")
                    .off("click")
                    .click(() => {
                        tryModifyOffset(-1);
                    });
                $("#make-later")
                    .off("click")
                    .click(() => {
                        tryModifyOffset(1);
                    });
                dialog.isValid = false;
            });
    }

    saveScenario() : void {
        Common.makePostRequest(
            "/Scenarios/Save",
            {
                contextid: this.contextId, 
            },
            () => {
                Common.showNotice(`Scenario saved.`);
                this.isDirty = false;
                this.updateControls();
                $("#results").html("");
                this.loadResults();
            },
            () => {
                Common.showAlert("Could not save.");
            }
        );
    }

    private readonly contextId: string;

    updateWeekOffsetText() {
        $(".week-offset")
            .each((index, element) => {
                var $element = $(element as any);

                let offsetValue = parseInt(
                    $element.data("value"),
                    10
                );

                $element
                    .text(this.describeWeekOffset(offsetValue));
            });
    }

    describeWeekOffset(offset: number): string {
        if (offset === 0) {
            return "no offset";
        }
        return `${Math.abs(offset)} week${Math.abs(offset) !== 1 ? "s" : "" } ${offset < 0 ? "earlier" : "later"}`;
    }

    updateControls() {
        if (this.isDirty) {
            $("#save-scenario").show();
            $("#modified-flag").show();
            $("#set-as-active").hide();
            $("#duplicate-scenario").hide();
            $((window) as any).on("beforeunload", () => { return "You have unsaved scenario." });
        } else {
            $("#save-scenario").hide();
            $("#modified-flag").hide();
            $("#set-as-active").show();
            $("#duplicate-scenario").show();
            $((window) as any).off("beforeunload");
        }
    }
}
