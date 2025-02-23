import * as Common from "./common";
import ModalDialog from "./modaldialog";

export default class Algorithms {
    constructor(contextId: string, index: number, offset: number) {
        $("#apply-and-stay").click(() => this.applyAndStay());

        $("#cancel-and-back").click(() => this.goBack());

        this.offset = offset;
        this.contextId = contextId;
        this.index = index;

        let hasActuals = $(".algorithm-values:last-child[data-is-actual='true']").length > 0;
        let foundActuals = false;
        console.log(`hasActuals: ${hasActuals}`);

        $(".algorithm-values:last-child")
            .each((index, element) => {
                let $td = $(element as any);
                console.log(`row: ${index}`);
                if (!hasActuals || (foundActuals && !$td.data("is-actual"))) {
                    $td.html(`<input class="text-right adjusted-editor" type="text" value="${$td.text()}" size="10"/>`);
                }
                if (hasActuals && $td.data("is-actual")) {
                    console.log("found actuals");
                    foundActuals = true;
                }
            });

        $(".adjusted-editor")
            .on("change", (event) => {
                $(event.delegateTarget as any)
                    .val((index, value) => {
                        return value
                            .replace(/\D/g, "")
                            .replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                });
            });
    }

    private offset: number;

    private index: number;

    private contextId: string;

    applyAndStay() {
        let adjustedvalues = $(".adjusted-editor")
            .toArray()
            .map((input, index) => {
                let $input = $(input as any);
                return $input.parents("td").first().data("is-actual")
                    ? ""
                    : $input.val().toString().replace(/\D/g, "");
            })
            .join(",");

        let week = $(".adjusted-editor")
            .parents("tr")
            .find("td")
            .first()
            .text();

        let algorithm = $("input:checked")
            .data("algorithm");

        Common.makePostRequest(
            `/Scenarios/Settings`,
            {
                contextid: this.contextId,
                index: this.index,
                algorithm,
                week,
                adjustedvalues
            },
            () => {
                //this.goBack();
                // stay!
                Common.showNotice("Changes applied. Don't forget to save the scenario!");
            },
            () => {
                $("#error").show();
            });
    }

    goBack() {
        window.location
            .replace(
                $("#control-link")
                .attr("href")
        );
    }
}
