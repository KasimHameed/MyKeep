$('.add-checklist').on('click', evt => {
    console.dir(evt);
    const target = $(evt.currentTarget);
    const color = target.data("color");
    $.ajax({
        url: "/api/checklist",
        method: "post",
        contentType: "application/json",
        data: JSON.stringify({
            id: crypto.randomUUID(),
            startingColor: color
        })
    }).then((_1,_2,jqxhr) => {
        window.location = jqxhr.getResponseHeader("Location");
    })
})