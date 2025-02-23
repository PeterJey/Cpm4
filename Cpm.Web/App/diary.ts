import * as Common from "./common";
import ModalDialog from "./modaldialog";

interface IUploadInfo {
    url: string;
    id: string;
}

export default class Diary {
    constructor(
        $dayContainer: JQuery<HTMLElement>, 
        onSelectionChanged: () => void,
        onModified: () => void
    ) {
        this.$dayContainer = $dayContainer;

        this.onSelectionChanged = onSelectionChanged;

        this.onModified = onModified;
    }

    getDate(): string {
        return this.date;
    }

    getFieldId(): string {
        return this.fieldId;
    }

    private $dayContainer: JQuery<HTMLElement>;

    private onSelectionChanged: (fieldId: string, date: string) => void;
    
    private onModified: (fieldId: string, date: string) => void;

    private fieldId: string;
    
    private date: string;

    public selected(fieldId: string, date: string) {
        this.fieldId = fieldId;
        this.date = date;

        this.loadDay();

        this.onSelectionChanged(fieldId, date);
    }

    public loadDay() {
        Common.loadContent(
            `/Diary/Details?fieldId=${this.fieldId}&day=${this.date}`,
            this.$dayContainer,
            () => {
                $("#plan-picking").click(() => this.planPicking());
                $("#record-harvest").click(() => this.recordHarvest());
                $("#edit-note").click(() => this.editNote());
                $("#delete-note").click(event => this.deleteNote());

                $(".delete-picture").click(event => this.deletePicture(event));

                $(".add-picture-btn").click(event => {
                    $("#file-input").click();
                    event.preventDefault();
                });
                $("#file-input").change(event => {
                    $("#upload-form").submit();
                    event.preventDefault();
                });
                $("#upload-form")
                    .submit(event => this.uploadPicture(event));

                $(".note-picture img").each((index, element) => this.configurePictureAutoLoad($(element as any)));
            });
    }

    deletePicture(event: JQuery.Event) {
        let dialog = new ModalDialog();

        dialog.confirmDanger(
            "Are you sure you want to delete the picture?",
            "Deleting the picture",
            "Delete",
            "Keep",
            () => {
                Common.makePostRequest(
                    "/Diary/DeletePicture",
                    {
                        fieldid: this.fieldId,
                        day: this.date,
                        id: $(event.delegateTarget as any).data("pictureid")
                    },
                    () => {
                        this.onModified(this.fieldId, this.date);
                        this.loadDay();
                        dialog.hide();
                    },
                    () => {
                        dialog.message = "Could not delete the picture.";
                    });
            });
    }

    deleteNote() {
        let dialog = new ModalDialog();

        dialog.confirmDanger(
            "Are you sure you want to delete the note?",
            "Deleting the note",
            "Delete",
            "Keep",
            () => {
                Common.makePostRequest(
                    "/Diary/DeleteNote",
                    {
                        fieldid: this.fieldId,
                        day: this.date,
                    },
                    () => {
                        this.onModified(this.fieldId, this.date);
                        this.loadDay();
                        dialog.hide();
                    },
                    () => {
                        dialog.message = "Could not delete the note.";
                    });
            });
    }

    uploadPicture(event: JQuery.Event) {
        let form = new FormData($("#upload-form")[0] as any);

        let dialog = new ModalDialog();

        dialog.showProgress(
            "Uploading",
            "Close",
            (setMessage, setProgress) => {
                dialog.isBusy = true;
                setMessage("Uploading picture");

                //let x = 0;
                //let iteration = () => {
                //    if (x < 100) {
                //        x = x + 5;
                //        setProgress(x);
                //        window.setTimeout(iteration, 500);
                //    } else {
                //        dialog.hide();
                //    }
                //};

                //window.setTimeout(iteration, 500);

                $.ajax({
                    type: "POST",
                    url: "/Diary/UploadPicture",
                    contentType: false,
                    data: form,
                    processData: false,
                    xhr: () => {
                        let myXhr = $.ajaxSettings.xhr();
                        if (myXhr.upload) {
                            myXhr.upload.addEventListener(
                                "progress",
                                ev => {
                                    let percent = Math.round(ev.loaded / ev.total * 100);
                                    setProgress(percent);
                                    if (percent >= 100) {
                                        setMessage("Now processing picture, please wait...");
                                    }
                                },
                                false);
                        }
                        return myXhr;
                    },
                    success: data => {
                        dialog.hide();
                        this.onModified(this.fieldId, this.date);
                        this.loadDay();
                    },
                    error: data => {
                        dialog.message = "Could not upload the picture.";
                        dialog.isBusy = false;
                    }
                });
            });

        event.preventDefault();
    }

    editNote() {
        let dialog = new ModalDialog();
        dialog.showElement(
            $("#edit-note-template"),
            () => {
                Common.makePostRequest(
                    "/Diary/SaveNote",
                    {
                        fieldId: this.fieldId,
                        day: this.date,
                        note: $("#note-text").val()
                    },
                    () => {
                        dialog.hide();

                        this.loadDay();

                        this.onModified(this.fieldId, this.date);
                    },
                    () => {
                        dialog.message = "Could not save the comment.";
                    });
            },
            () => {
                dialog.hide();
            },
            () => {
                $("#note-text").val($("#originalNoteText").text());
            });
    }

    recordHarvest() {
        let dialog = new ModalDialog();
        dialog.showElement(
            $("#edit-harvest-template"),
            () => {
                Common.makePostRequest(
                    "/Diary/SaveHarvest",
                    {
                        fieldId: this.fieldId,
                        day: this.date,
                        value: $("#new-harvest-weight").val().toString().replace(/,/g, ""),
                        last: $("#last-harvest-day").prop("checked") ? "true" : "false"
                    },
                    () => {
                        dialog.hide();

                        this.loadDay();

                        this.onModified(this.fieldId, this.date);
                    },
                    () => {
                        dialog.message = "Could not record the harvest.";
                    });
            },
            () => {
                dialog.hide();
            },
            () => {
                $("#new-harvest-weight").val($("#harvest-weight").text());
            });
    }

    planPicking() {
        let dialog = new ModalDialog();
        dialog.showElement(
            $("#plan-picking-template"),
            () => {
                Common.makePostRequest(
                    "/Diary/SavePickPlan",
                    {
                        fieldId: this.fieldId,
                        day: this.date,
                        value: $("#new-planned-weight").val().toString().replace(/,/g, ""),
                    },
                    () => {
                        dialog.hide();

                        this.loadDay();

                        this.onModified(this.fieldId, this.date);
                    },
                    () => {
                        dialog.message = "Could not plan the pick.";
                    });
            },
            () => {
                dialog.hide();
            },
            () => {
                $("#new-planned-weight").val($("#planned-weight").text());
            });
    }

    configurePictureAutoLoad($el: JQuery<Node>) {
        let img = new Image();
        $el.data("retries", 0);
        img.onload = () => {
            $el.prop("src", img.src);
            $el.width("auto");
        };
        img.onerror = () => {
            let retries = $el.data("retries");
            if (retries < 10) {
                $el.data("retries", retries + 1);
                let delay = Math.min(Math.pow(2, retries) * 200, 2000);
                window.setTimeout(() => img.src = $el.data("src"), delay);
            }
        };
        img.src = $el.data("src");
        img.onerror(null);
    }
}
