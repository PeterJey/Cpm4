
export function truncateText(text: string, maxLength: number, useWordBoundary: boolean) {
    if (text.length <= maxLength) {
        return text; 
    }

    const subString = text.substr(0, maxLength);

    return (useWordBoundary
        ? subString.substr(0, subString.lastIndexOf(" "))
        : subString.substr(0, maxLength - 1)) + "&hellip";
};

export function makePostRequest(
    url: string, 
    data: any, 
    success: (data) => void = null, 
    error: (data) => void = null
): void {

    const token = ((($("[name='__RequestVerificationToken']")[0]) as any) as HTMLInputElement).value;
    const dataWithAntiforgeryToken = $.extend(data, { '__RequestVerificationToken': token });

    $.ajax({
        url: url,
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: success,
        error: error
    });
}

export function makeGetRequest(
    url: string,
    success: (data) => void = () => { },
    error: () => void = () => { }
): void {
    $.ajax({
        url: url,
        type: "GET",
        data: {},
        success: success,
        error: error
    });
}

function showGenericAlert(text: string, alertClass: string) {
    let $alerts = $(".body-content .alert");

    let replaceAlerts = () => {
        $(".body-content .alert").remove();
        $(".page-header")
            .after(
                `<div class="alert ${alertClass
                } alert-dismissible"><button type="button" class="close" data-dismiss="alert">&times;</button>${
                text}</div>`);
    };

    if ($alerts.length > 0) {
        $alerts
            .hide("fast", () => replaceAlerts());
    } else {
        replaceAlerts();
    }
}

export function showAlert(text: string) {
    showGenericAlert(text, "alert-danger");
}

export function showNotice(text: string) {
    showGenericAlert(text, "alert-info");
}

export function loadContent(
    url: string, 
    $container: JQuery<HTMLElement>,
    onSuccess: () => void = () => {},
    attempt: number = 1
) {
    $container.html(spinnerTemplate);
    console.log(`attempt no ${attempt}`);
    makeGetRequest(
        url,
        (data) => {
            $container.html(data);
            $container.show();

            onSuccess();
        },
        () => {
            $container.html(retryTemplate);

            $container
                .find(".retry-button")
                .on("click", (event) => {
                    $(event.target).off("click");
                    loadContent(url, $container, onSuccess, ++attempt);
                });

            $container.show();
        }
    );
}

const retryTemplate = `There was a problem loading the content. <button class="btn btn-default btn-sm retry-button">Retry</button>`;

const spinnerTemplate = "Loading...";