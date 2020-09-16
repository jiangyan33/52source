$(function () {
    // 获取路由参数
    const dict = getUrlParams(window.location.href);
    $.get(`/api/contents/${dict.id}`, function (data) {
        if (data.code === 0) {
            let navigation = data.data;
            let html = "";

            let contentArr = navigation.currentPage.remark.split('\n');

            for (let item of contentArr) {

                html += `<p>${item.trim()}</p>`;
            }
            html += "<br>";
            if (navigation.previousPage) {
                html += `<h3><a href='./content.html?id=${navigation.previousPage.id}'>上一章：${navigation.previousPage.chapter}</a></h3>`;
            }
            if (navigation.nextPage) {
                html += `<br><h3><a href='./content.html?id=${navigation.nextPage.id}'>下一章：${navigation.nextPage.chapter}</a></h3>`;
            }
            $('#content').html(html);

            $('.is-text').html(`<span>${navigation.currentPage.chapter}</span>`);
        }
    });
})