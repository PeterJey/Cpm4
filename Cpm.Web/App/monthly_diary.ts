import * as Common from "./common";
import ModalDialog from "./modaldialog";
import Diary from "./diary";

export default class MonthlyDiary {
    constructor(fieldId: string) {
        this.diary = new Diary(
            $("#details"),
            () => this.setSelection(),
            () => this.reloadCalendar()
        );

        this.fieldId = fieldId;

        $("#firstNoteButton").click(() => this.loadCalendar("firstnote"));
        $("#firstHarvestButton").click(() => this.loadCalendar("firstharvest"));
        $("#previousButton").click(() => this.loadCalendar((--this.position).toString(10)));
        $("#todayButton").click(() => this.loadCalendar(""));
        $("#nextButton").click(() => this.loadCalendar((++this.position).toString(10)));
        $("#lastHarvestButton").click(() => this.loadCalendar("lastharvest"));
        $("#lastNoteButton").click(() => this.loadCalendar("lastnote"));

        $("#todayButton").click();
    }

    private diary: Diary;

    private position: number = 0;

    private fieldId: string;

    private setSelection() {
        $(".diary-day-selected")
            .removeClass("diary-day-selected");

        $(`.dc-day[data-date='${this.diary.getDate()}']`)
            .addClass("diary-day-selected");
    }

    reloadCalendar() {
        this.loadCalendar(this.position.toString(10));
    }

    loadCalendar(which: string) {
        Common.loadContent(
            `/Diary/Calendar?fieldId=${this.fieldId}&which=${which}`,
            $("#calendar"),
            () => {
                this.position = parseInt($("#position").text(), 10);

                $(".dc-day").click((event) => {
                    let $div = $(event.delegateTarget as any);
                    this.diary.selected(this.fieldId, $div.data("date"));
                });

                this.setSelection();
            });
    }
}
