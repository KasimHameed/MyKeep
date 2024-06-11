$('#add-checklist').on('click', _ => {
    $.ajax({
        url: "/api/checklist",
        method: "post",
        contentType: "application/json",
        data: JSON.stringify({
            id: crypto.randomUUID(),
        })
    }).then((_1,_2,jqxhr) => {
        window.location = jqxhr.getResponseHeader("Location");
    })
})