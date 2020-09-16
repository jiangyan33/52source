
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
            listNode.html(resHtml);
            SetPagination(categoryId, currentPage, data.data.pages);
        }
    });
};
function loadCategoryTexts(parentId, currentPage = 1) {
    $.get(`/api/contents?pageNum=${currentPage}&parentId=${parentId}`, function (data) {
        if (data.code === 0) {

            var resHtml = "<dl><dt>文学专区</dt>";
            if (parentId) {
                var resHtml = `<dl><dt>${data.data.data[0].parentName}</dt>`;
            }
            data.data.data.map(it => it.createDate = new Date(it.createDate).toLocaleDateString());
            var res = data.data.data;
            for (const item of res) {
                if (item.parentId === 0) {
                    // 进入子集
                    resHtml += `<dd>
                                  <span>${item.createDate}</span> <a href="./category.html?parentId=${item.id}&categoryId=1">${item.chapter}</a>
                                </dd>`;
                } else {
                    // 进入详情
                    resHtml += `<dd>
                                    <span>${item.createDate}</span> <a href="./content.html?id=${item.id}">${item.chapter}</a>
                                </dd>`;
                }
            }
            resHtml += "</dl>";
            var listNode = $("div.docify-justify-list");
            listNode.html(resHtml);
            // SetPagination(categoryId, currentPage, data.data.pages);
        }
    });
};
$(function () {
    // 分类详情

    // 获取路由参数
    const dict = getUrlParams(window.location.href);
    if (+dict.categoryId === 1) {
        // 文学专区特殊处理
        let parentId = 0;
        if (dict.parentId) parentId = dict.parentId;
        loadCategoryTexts(parentId);
    } else {
        loadCategoryVideos(dict.categoryId);
    }
});

