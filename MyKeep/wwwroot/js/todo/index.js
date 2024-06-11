const checkListTitle = $('#checkListTitle');
const pending = $('.pending');
const pendingCheckbox = $('.pending input[type=checkbox]')
const pendingLabel = $('.pending > label');
const pendingItems = $('#pendingItems');
const completedItems = $('#completedItems');
const main = $('main');
checkListTitle.on('focus', evt => {
    const target = $(evt.target);
    const originalTitle = target.data("originalTitle");
    if (originalTitle === "") target.text("");
    target.removeClass('text-muted');
}).on('keydown', evt => {
    const target = $(evt.target);
    if (evt.keyCode === 27) {
        $(evt.target).trigger("blur");
        let title = target.data('originalTitle');
        if (title === "") {
            checkListTitle.addClass("text-muted");
            title = "Untitled";
        }
        checkListTitle.text(title);
    } else if (evt.keyCode === 13) {
        $(evt.target).trigger('blur');
    }
}).on('blur', evt => {
    const target = $(evt.target);

    const payload = {id: checkListTitle.data('id'), title: target.text()};

    if (payload.title === "") {
        checkListTitle.addClass('text-muted');
        checkListTitle.text('Untitled');
    }
    $.ajax({
        url: "/api/checklist/title", method: "patch", contentType: "application/json", data: JSON.stringify(payload)
    }).then(_ => {
        checkListTitle.data('originalTitle', payload.title);
    });
});

pendingItems.on('click', '.pending', evt => {
    evt.preventDefault();
    $(evt.target).find('label').trigger('focus');
});
pendingItems.on('click', '.pending input[type=checkbox]', evt => evt.stopPropagation());

pendingItems.on('keydown', '.pending > label', evt => {
    const target = $(evt.target);
    if (evt.keyCode === 13) {
        target.trigger('blur');
    }
})

pendingItems.on('blur', '.pending > label', evt => {
    const target = $(evt.target);
    const pending = target.closest('.pending');
    const key = pending.data('key');
    $.ajax({
        url: "/api/checklist",
        method: "patch",
        contentType: "application/json",
        data: JSON.stringify({
            id: checkListTitle.data('id'),
            key: key,
            text: target.text(),
            isCompleted: false
        })
    })
})
pendingItems.on('dragstart', 'i[draggable=true]', evt => {
    let index = 0;
    
    const target = $(evt.target);
    target.addClass('dragging');
    //evt.dataTransfer.setDragImage(new Image())
    pendingItems.data("target", target);
    
    for (let elem of pendingItems.find('.pending')) {
        elem = $(elem)
        if (!elem.is('.pending')) continue;
        if (index === 0) {
            elem.prepend($('<div class="dragTarget position-absolute w-100 h-100 top" style="bottom: 35%"></div>'))
        }
        elem.prepend($('<div class="dragTarget position-absolute w-100 h-100 bottom" style="bottom: -50%"></div>'))
        index++;
    }
})

pendingItems.on('dragover', '.dragTarget', evt => {
    evt.preventDefault();
    $(evt.target).addClass('targetted');
})

pendingItems.on('dragleave', '.dragTarget', evt => {
    evt.preventDefault();
    $(".targetted").removeClass('targetted');
})

pendingItems.on('dragend', 'i[draggable=true]', evt => {
    const targetted = $(".targetted");
    if (targetted.length === 0) return;
    const isTop = targetted.is('.top');
    const targetParent = targetted.parent();
    const dragging = $(".dragging").removeClass("dragging").parent();
    const dragKey = dragging.data("key");
    const targetKey = isTop ? undefined : targetParent.data('key');
    $('.dragTarget').remove();
    $.ajax({
        url: "/api/checklist/reorder",
        method: "patch",
        contentType: "application/json",
        data: JSON.stringify({
            id: checkListTitle.data('id'),
            key: dragKey,
            afterKey: targetKey
        })
    }).then(_ => {
        dragging.detach();
        if (targetKey === undefined) {
            dragging.prependTo(pendingItems);
        } else {
            //pendingItems.children[targetKey].after(dragging);
            dragging.insertAfter(`#pendingItems [data-key=${targetKey}]`);
        }
    })
})


main.on('click', '.remove-item', evt => {
    const target = $(evt.target);
    const parent = target.parent();
    const key = parent.data('key');
    $.ajax({
        url: "/api/checklist/item",
        method: "delete",
        contentType: "application/json",
        data: JSON.stringify({
            id: checkListTitle.data('id'),
            key: key,
            isCompleted: parent.is('.completed')
        })
    }).then(_ => {
        parent.remove();
    })
});

pendingItems.on('change', 'input[type=checkbox]', evt => {
    const target = $(evt.target);
    const pending = target.closest('.pending');
    const key = pending.data('key');
    $.ajax({
        url: "/api/checklist/complete",
        method: "put",
        contentType: "application/json",
        data: JSON.stringify({
            id: checkListTitle.data('id'),
            key: key
        })
    }).then(_ => {
        pending.removeClass('pending').addClass('completed');
        pending.appendTo(completedItems);
        pending.prop('draggable', false);
    })
})

completedItems.on('change', 'input[type=checkbox]', evt => {
    const target = $(evt.target);
    const completed = target.closest('.completed');
    const key = completed.data('key');
    $.ajax({
        url: "/api/checklist/uncomplete",
        method: "put",
        contentType: "application/json",
        data: JSON.stringify({
            id: checkListTitle.data('id'),
            key: key
        })
    }).then(_ => {
        completed.removeClass('completed').addClass('pending');
        completed.appendTo(pendingItems);
        completed.prop('draggable', true);
    })
})

$('#addPending').on('click', evt => {
    $.ajax({
        url: "/api/checklist/add",
        method: "post",
        contentType: "application/json",
        data: JSON.stringify({id: checkListTitle.data('id')})
    }).then(data => {
        const newLabel = `<div class="pending position-relative" data-key="${data.key}"><i class="fa fa-grip-vertical fa-fw me-1" draggable="true"></i> <input type="checkbox"><label contenteditable="true"></label><i class="fa fa-close position-absolute end-0 pe-2 pt-1 remove-item" role="button"></i></div>`;
        pendingItems.append($(newLabel));
    })
});