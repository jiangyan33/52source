function SetTextPagination(parentId, currentPage, totalPages) {
    let navStr = "";
    // 处理分页信息
    if (currentPage > 1) {
        navStr += `<span class=first> <a href='javascript:void(0)' onclick="loadCategoryTexts('${parentId}',1)">首页</a> </span>\n<span class=prev> <a href='javascript:void(0)' rel=prev   onclick="loadCategoryTexts('${parentId}',${currentPage - 1})">上一页</a> </span>`;
    }

    if (currentPage <= 6) {
        // 前6页，当前页前展示效果一样
        for (let i = 1; i <= currentPage; i++) {
            if (i === currentPage) {
                navStr += `<span class="page current"> ${currentPage} </span>`;
            } else {
                navStr += `<span class=page> <a href='javascript:void(0)' rel=next onclick="loadCategoryTexts('${parentId}',${i})">${i}</a> </span>`;
            }
        }
        // 处理当前页后面的数据
        if (totalPages - 6 <= currentPage) {
            // 后6页，当前页后展示效果一样
            // 循环次数
            let length = totalPages - currentPage;
            for (let i = 1; i <= length; i++) {
                navStr += `<span class=page> <a href='javascript:void(0)' rel=next    onclick="loadCategoryTexts('${parentId}',${i + currentPage})">${i + currentPage}</a> </span>`;
            }
        } else {
            let length = currentPage + 4;
            navStr += `<span class="page gap">&hellip;</span>`;
            for (let i = currentPage + 1; i <= length; i++) {
                navStr += `<span class=page> <a href='javascript:void(0)' rel=next onclick="loadCategoryTexts('${parentId}',${i})">${i}</a> </span>`;
            }
            navStr += `<span class="page gap">&hellip;</span>`;
        }

    } else if (totalPages - 6 <= currentPage) {
        // 后6页，当前页后展示效果一样
        navStr += `<span class="page gap">&hellip;</span>`;
        for (let i = currentPage - 4; i < currentPage; i++) {
            navStr += `<span class=page> <a href='javascript:void(0)' rel=next onclick="loadCategoryTexts('${parentId}',${i})">${i}</a> </span>`;
        }
        let length = totalPages - currentPage;
        for (let i = 1; i <= length; i++) {
            if (i === currentPage) {
                navStr += `<span class="page current"> ${currentPage} </span>`;
            } else {
                navStr += `<span class=page> <a href='javascript:void(0)' rel=next onclick="loadCategoryTexts('${parentId}',${i + currentPage})">${i + currentPage}</a> </span>`;
            }
        }

    } else {
        // 当前页前4后4加... 包括首页尾页
        navStr += `<span class="page gap">&hellip;</span>`;
        for (let i = currentPage - 4; i < currentPage; i++) {
            navStr += `<span class=page> <a href='javascript:void(0)' rel=next onclick="loadCategoryTexts('${parentId}',${i})">${i}</a> </span>`;
        }
        let length = currentPage + 5;
        for (let i = currentPage; i < length; i++) {
            if (i === currentPage) {
                navStr += `<span class="page current"> ${currentPage} </span>`;
            } else {
                navStr += `<span class=page> <a href='javascript:void(0)' rel=next onclick="loadCategoryTexts('${parentId}',${i})">${i}</a> </span>`;
            }
        }
        navStr += `<span class="page gap">&hellip;</span>`;
    }

    if (currentPage < totalPages) {
        navStr += `<span class=next> <a href='javascript:void(0)' onclick="loadCategoryTexts('${parentId}',${currentPage + 1})" rel=next>下一页</a> </span>\n <span class=last> <a href='javascript:void(0)' onclick="loadCategoryTexts('${parentId}',${totalPages})">末页</a> </span>`;
    }
    var navNode = $("nav.pagination");
    navNode.html(navStr);
}