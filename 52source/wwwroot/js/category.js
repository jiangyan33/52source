
function loadCategoryVideos(categoryId, currentPage = 1) {
    $.get(`/api/categories/${categoryId}/videos?pageNum=${currentPage}`, function (data, status) {
        if (data.code === 0) {
            var resHtml = "";
            var res = data.data.data;
            for (const item of res) {
                resHtml += `<div class="post-item is-item ovh docify-card">
                                <a href="./detail.html?videoId=${item.id}">
                                    <figure class="webkit-sassui-scaleable-image docify-ratio-image">
                                        <img src="${item.pic}" title="${item.name}"
                                            class="is-scaleable is-ratio" />
                                        <figcaption class=is-caption>
                                            <strong>发布日期:</strong> <span>${item.createDate}</span>
                                        </figcaption>
                                    </figure>
                                </a>
                                <div class="px10 text-justify lh-20 mb10_ mb__ is-body">
                                    <h3 class="b lc-2 my10 ovh" style="height: 40px"> <a href="./detail.html?videoId=${item.id}"> <i class="n iconfont icon-store"></i>
                                    ${item.name} </a> </h3>
                                    <div class="row row-justify-between row-center c-6 pb10">
                                        <div class=is-left> <span class=f12> <i class="f12 iconfont icon-assessedbadge"></i> <span>热度:</span>
                                            </span> <span class="f14 c-blue">${item.hot}</span> </div>
                                    </div>
                                </div>
                            </div>`;
            }
            var listNode = $("div.docify-justify-list");
            listNode.empty();
            listNode.append(resHtml);
            SetPagination(categoryId, currentPage, data.data.pages);
        }
    });
};
$(function () {
    // 分类详情

    // 获取路由参数
    const dict = getUrlParams(window.location.href);

    loadCategoryVideos(dict.categoryId);
});

