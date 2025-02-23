import * as Common from "./common";

export default class ModalDialog {

    constructor() {
        if ($("#mbox").length === 0) {
            $(".body-content")
                .prepend(this.dialogTemplate);
        }

        this.$dialog = $("#mbox");
    }

    private $dialog: JQuery<HTMLElement>;

    private $elementsParent: JQuery<HTMLElement>;
    
    private onDone: () => void;

    private _isValid: boolean = true;
    get isValid(): boolean {
        return this._isValid;
    }
    set isValid(value:boolean) {
        if (this._isValid === value) {
            return;
        };
        this._isValid = value;
        this.updateButtons();
    }

    private _isBusy: boolean = false;
    get isBusy(): boolean {
        return this._isBusy;
    }
    set isBusy(value:boolean) {
        if (this._isBusy === value) {
            return;
        };
        this._isBusy = value;
        this.updateButtons();
    }

    private _message: string;
    get message(): string {
        return this._message;
    }
    set message(value:string) {
        if (this.message === value) {
            return;
        };
        this._message = value;

        this.$dialog
            .find("#mbox-message")
            .empty()
            .append(
            $(this._message ? ("<br/><p>" + this._message.split("\n").join("<br/>")) + "</p>" : "")
        );
    }

    updateButtons() {
        this.$dialog
            .find("#mbox-confirm")
            .prop("disabled", this._isValid && !this._isBusy ? "" : "disabled");

        this.$dialog
            .find("#mbox-cancel")
            .prop("disabled", !this._isBusy ? "" : "disabled");

        if (this._isBusy) {
            this.$dialog
                .find("#mbox-spinner")
                .show();
        } else {
            this.$dialog
                .find("#mbox-spinner")
                .hide();
        }
    }

    confirmPrimary(
        message: string, 
        title: string, 
        confirmText: string, 
        cancelText: string, 
        onConfirm: () => void, 
        onCancel: () => void = () => this.hide()
    ) {
        this.showElement(
            $(`<p class="mbox-focus" data-confirm="${confirmText}" data-cancel="${cancelText}" data-title="${title}">${message}</p>`), 
            onConfirm, 
            onCancel);
    }

    confirmDanger(
        message: string, 
        title: string, 
        confirmText: string, 
        cancelText: string, 
        onConfirm: () => void, 
        onCancel: () => void = () => this.hide()
    ) {
        this.showElement(
            $(`<p class="mbox-focus" data-confirm="${confirmText}" data-cancel="${cancelText}" data-title="${title}">${message}</p>`), 
            onConfirm, 
            onCancel,
            () => {},
            () => { },
            $(this.dangerButtonsTemplate));
    }

    showProgress(
        title: string, 
        cancelText: string, 
        onRun: (setMessage: (message: string) => void, setPercent: (percent: number)=>void) => void,
        onCancel: () => void = () => this.hide()
    ) {
        const template = `<p class="mbox-focus" data-cancel="${cancelText}" data-title="${title}">` +
            `<div id="mbox-progress-message"></div>` +
            `<div class="progress">` +
            `<div class="progress-bar" role="progressbar" width="0">` +
            `</div>` +
            `</div>` +
            `</p>`;

        let updatePercent = (percent: number) => {
            let val = `${percent || 0}%`;
            console.log(val);
            $("#mbox-content .progress-bar")
            .width(val)
            .text(val);
        }

        this.showElement(
            $(template), 
            () => {}, 
            onCancel,
            () => {
                updatePercent(0);
                onRun(
                    message => {
                        $("#mbox-progress-message")
                            .text(message);
                    },
                    updatePercent);
            },
            () => {},
            $(this.progressButtonsTemplate));
    }

    showElement(
        $element: JQuery<HTMLElement>, 
        onConfirm: () => void, 
        onCancel: () => void = () => this.hide(),
        onReady: () => void = () => {},
        onDone: () => void = () => {},
        $buttons: JQuery<HTMLElement> = $(this.primaryButtonsTemplate) 
    ) {
        this.onDone = onDone;
        this.$dialog.find(".modal-title").text($element.data("title"));
        this.$elementsParent = $element.parent();
        this.$dialog
            .find("#mbox-content")
            .empty()
            .append($element);
        this.$dialog
            .find("#mbox-footer")
            .empty()
            .append($buttons);
        this.$dialog.find("#mbox-cancel")
            .text($element.data("cancel") || "Cancel")
            .click(() => {
                onCancel();
            });
        this.$dialog.find("#mbox-confirm")
            .text($element.data("confirm") || "Confirm")
            .click(() => {
                onConfirm();
            });
        this.$dialog.find(".modal-footer button")
            .prop("disabled", false);

        this.$dialog.find("input.mbox-number")
            .addClass("text-right")
            .on("input", (event) => this.reformatNumber($(event.delegateTarget as any)));

        this.message = null;
        this.isBusy = false;
        this.updateButtons();
        onReady();

        this.$dialog.find("input.mbox-number")
            .each((index, element) => this.reformatNumber($(element as any)));

        this.$dialog.on("shown.bs.modal",
            () => {
                this.$dialog
                    .off("shown.bs.modal");
                this.$dialog.find(".mbox-focus").focus();
            });

        (this.$dialog as any).modal();
    }

    reformatNumber($element: JQuery) {
        $element
            .val((index, value) => {
                return value
                    .replace(/\D/g, "")
                    .replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            });
    }

    hide() {
        this.$dialog.find(".modal-footer button")
            .off("click");

        this.$dialog.find("input.mbox-number")
            .off("input");

        this.$dialog.on("hidden.bs.modal", () => {
            this.$dialog
                .off("hidden.bs.modal");

            this.isBusy = false;

            if (this.$elementsParent) {
                this.$elementsParent
                .append(this.$dialog
                    .find("#mbox-content")
                    .children());
            } else {
                this.$dialog
                    .find("#mbox-content")
                    .children()
                    .remove();
            }
            this.onDone();
        });

        (this.$dialog as any).modal("hide");
    }

    private dangerButtonsTemplate = 
            '<button type="button" class="btn btn-default" id="mbox-cancel"></button>' +
            '<button type="button" class="btn btn-danger" id="mbox-confirm"></button>';

    private primaryButtonsTemplate = 
            '<button type="button" class="btn btn-default" id="mbox-cancel"></button>' +
            '<button type="button" class="btn btn-primary" id="mbox-confirm"></button>';

    private progressButtonsTemplate = 
            '<button type="button" class="btn btn-default" id="mbox-cancel"></button>';

    private dialogTemplate = 
                '<div class="modal fade" tabindex="-1" role="dialog" id="mbox" data-backdrop="static">' +
                '<div class="modal-dialog" role="document">' +
                '<div class="modal-content">' +
                '<div class="modal-header">' +
                '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                '<h4 class="modal-title"></h4>' +
                "</div>" +
                '<div class="modal-body">' +
                '<div id="mbox-content">' +
                "</div>" +
                '<div id="mbox-message" class="text-danger">' +
                "</div>" +
                "</div>" +
                '<div class="modal-footer">' +
                '<span id="mbox-spinner">Please wait <span class="fa fa-spinner fa-spin fa-lg"></span></span>' +
                '<div id="mbox-footer">' +
                "</div>" +
                "</div>" +
                "</div>" +
                "</div>" +
                "</div>";
}