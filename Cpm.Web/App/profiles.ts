import * as Common from "./common";
import ModalDialog from "./modaldialog";

export default class Profiles {
    constructor() {
        $(".profile-list > ul > li")
            .each((index, element) => {
                const $profileLi = $(element as any);

                const valueItems = $profileLi.find(".profile-point-value")
                    .toArray();

                const max = Math.max(...valueItems.map(value => parseFloat(value.textContent)));

                valueItems
                    .forEach(item => {
                        const value = parseFloat(item.textContent);
                        $(item as any).parent().find(".profile-point-bar")
                            .width(
                                max > 0 ? Math.round(value / max * 100).toString(10) + "%" : "0"
                            );
                    });
            });

        $(".open-editor")
            .click(event => this.editVariant($(event.delegateTarget as any)));

        $(".delete-variant")
            .click(event => this.deleteVariant($(event.delegateTarget as any)));
    }

    editVariant($btn: JQuery<Node>) {
        const profileName = $btn.parent().data("profilename");
        const profileId = $btn.parent().data("profileid");

        const dialog = new ModalDialog();
        dialog.showElement(
            $("#edit-variant-template"),
            () => {
                this.validateVariant(
                    $("#data-box").val() as string,
                    data => {
                        console.log(`Updating ${profileId} with data:`);
                        console.log(data);
                        Common.makePostRequest(
                            "/Profiles/Save",
                            data,
                            () => {
                                dialog.hide();
                                location.reload();
                            },
                            (err) => dialog.message = `Could not save: ${Common.truncateText((err as JQueryXHR).responseText, 100, true)}`
                            );
                    },
                    err => {
                        dialog.message = err;
                    }
                );
            },
            () => { dialog.hide(); },
            () => {
                $("#profile-name-box").val(profileName);
                $("#starting-week-box").val($btn.parent().data("startingweek"));
                $("#extra-potential-box").val($btn.parent().data("extrapotential"));
                $("#description-box").val($btn.parent().data("description"));
                $("#comments-box").val($btn.parent().data("comments"));
                $("#season-box").val($btn.parent().data("season"));
                $("#quality-box").val($btn.parent().data("quality"));
                this.updateSeasonTypes();
                $("#season-type-box").val($btn.parent().data("seasontype"));

                if (profileName) {
                    // existing profile
                    const values = $btn.parent().find("li")
                        .toArray()
                        .map(el =>
                            $(el as any).find(".profile-point-value").text() +
                            " " +
                            $(el as any).find(".profile-point-prod").text()
                    );

                    $("#data-box").val(values.join("\n"));
                    $("#profile-id-box").val(profileId);
                } else {
                    $("#data-box").val("");
                    $("#profile-id-box").val("");
                }

                $("#season-box")
                    .change(() => this.updateSeasonTypes());
            });
    }

    updateSeasonTypes() {
        const season = $("#season-box").val();

        const container = $(`#season-${season}-items`)
            .data("items");

        console.log(container);

        const items = container ? container.split("|") as string[] : new Array<string>();

        $("#season-type-box")
            .empty()
            .append(
            items.map(val => {
                const option = new Option(val, val);
                option.value = val;
                return option;
            })
        );
    }

    deleteVariant($btn: JQuery<Node>) {
        const variantDesc = $btn.parent().data("seasons");
        const profileName = $btn.parent().data("profilename");
        const profileId = $btn.parent().data("profileid");

        const dialog = new ModalDialog();
        dialog.confirmDanger(
            `Delete the variant "${variantDesc}" of profile "${profileName}"?`,
            "Delete",
            "Delete",
            "Keep",
            () => {
                Common.makePostRequest(
                    "/Profiles/Delete",
                    { id: profileId },
                    () => {
                        dialog.hide();
                        location.reload();
                    },
                    () => {
                        dialog.message = "Could not delete.";
                    });
            });
    }

    validateVariant(
        rawValues: string,
        onSuccess: (data: any) => void,
        onError: (err: string) => void) {

        const errors = new Array<string>();

        const profileName = $("#profile-name-box").val();

        profileName || errors.push("Missing profile name");

        const startingWeek = parseInt($("#starting-week-box").val() as string, 10);

        (startingWeek && startingWeek > 0) || errors.push("Incorrect starting week");

        const extraPotentialRaw = $("#extra-potential-box").val() as string;
        const extraPotential = parseInt(extraPotentialRaw, 10) / 100;

        (extraPotentialRaw && !isNaN(extraPotential)) || errors.push("Incorrect extra potential value");

        const quality = $("#quality-box").val();

        quality || errors.push("Incorrect quality");

        const season = $("#season-box").val();

        season || errors.push("Incorrect season");

        const seasonType = $("#season-type-box").val();

        seasonType || errors.push("Incorrect season type");

        const points = ($("#data-box").val() as string || "")
            .split("\n")
            .map(line => line.split("\t").join(" ").split(" ").filter(v => v.trim()))
            .filter(ar => ar);

        if (points.length === 0) {
            errors.push("There are no data points");
        }

        const weights = new Array<number>();
        const productivity = new Array<number>();

        points
            .forEach((value, index) => {
                if (!value) {
                    errors.push(`Item at #${index} is empty`);
                } else if (value.length !== 2) {
                    errors.push(`Item at #${index} does not contain 2 elements`);
                } else {
                    const weight = parseFloat(value[0]);
                    const prod = parseFloat(value[1]);

                    if (!weight && weight !== 0) {
                        errors.push(`Item at #${index} weight is invalid`);
                    }

                    if (!prod && prod !== 0) {
                        errors.push(`Item at #${index} productivity is invalid`);
                    }

                    weights.push(weight);
                    productivity.push(prod);
                }
            });

        if (errors.length > 0) {
            onError(errors.join("\n"));
            return;
        }

        onSuccess({
            id: $("#profile-id-box").val(),
            profileName,
            startingWeek,
            extraPotential,
            quality,
            season,
            seasonType,
            weights,
            productivity,
            description: $("#description-box").val(),
            comments: $("#comments-box").val()
        });
    }
}
