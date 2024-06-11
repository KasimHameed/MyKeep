
$.ajax = (($oldAjax) => {
    const defaultSettings = { retries: [50, 100, 250], attempt: 0 };
    const stopCodes = [400, 401, 403, 404, 409];
    function check(data,status,_2) {
        const shouldRetry = status !== 'success' && status !== 'parsererror' && this.attempt < this.retries.length;

        // if we got an expected bad result, return immediately
        if (!shouldRetry || stopCodes.includes(data.status)) {
            if (this.noToast || status === 'success') return;
            if (data.responseJSON.type === "https://tools.ietf.org/html/rfc9110#section-15.5.1")
                toastr.error(data.responseJSON.detail);
            else
                toastr.error("An unexpected error occurred. Please reload the page and try again.")
            return;
        }

        if (shouldRetry) {
            setTimeout(() => $.ajax({...this, attempt: this.attempt + 1}), this.retries[this.attempt]);
        }
    }

    return settings => $oldAjax({...defaultSettings, ...settings }).always(check)
})($.ajax);