import * as Common from "./common";
import Diary from "./diary";

export default class WeeklyDiary {
    constructor(siteId: string, position: string) {
        this.diary = new Diary(
            $("#details"),
            () => this.setSelection(),
            () => this.reload()
        );

        this.position = parseInt(position, 10);

        this.siteId = siteId;

        $("#previousButton").click(() => this.load((--this.position).toString(10)));
        $("#todayButton").click(() => this.load(""));
        $("#nextButton").click(() => this.load((++this.position).toString(10)));

        this.reload();
    }

    private diary: Diary;

    private position: number;

    private siteId: string;

    private setSelection() {
        $(".diary-day-selected")
            .removeClass("diary-day-selected");

        $(`.dc-day[data-date='${this.diary.getDate()}'][data-fieldid='${this.diary.getFieldId()}']`)
            .addClass("diary-day-selected");
    }

    reload() {
        this.load(isNaN(this.position) ? "" : this.position.toString(10));
    }

    load(which: string) {
        Common.loadContent(
            `/Diary/WeekOverview?siteId=${this.siteId}&which=${which}`,
            $("#calendar"),
            () => {
                this.position = parseInt($("#position").text(), 10);

                $(".dc-day").click((event) => {
                    let $div = $(event.delegateTarget as any);
                    this.diary.selected($div.data("fieldid"), $div.data("date"));
                });

                this.setSelection();
            });
    }
}
