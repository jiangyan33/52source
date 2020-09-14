$(function () {
    // 加载分类
    $.get("/api/categories", function (data) {
        if (data.code === 0) {
            // 获取路由参数
            let indexCate = '<a class="px10" href="/"> <i class="f16 iconfont icon-warehouse-delivery"></i> 首页 </a>\n';
            let loginCate = '<a class="px10" href="../users/sign_in.html"> <i class="f16 iconfont icon-gerenzhongxin"></i> 登录/注册 </a>\n';
            const href = window.location.href;
            let isCate = false;
            if (href.indexOf('?') === -1) {
                if (href.indexOf('users') === -1) {
                    // 首页
                    indexCate = '<a class="px10 is-active" href="javascript:void(0)"> <i class="f16 iconfont icon-warehouse-delivery"></i> 首页 </a>\n';
                } else {
                    // 登录注册页面
                    loginCate = '<a class="px10 is-active" href="javascript:void(0)"> <i class="f16 iconfont icon-gerenzhongxin"></i> 登录/注册 </a>\n';
                }
            } else {
                // 分类页
                isCate = true;
            }
            var resHtml = indexCate + loginCate;
            if (isCate) {
                var dict = getUrlParams(href);
                for (const item of data.data) {
                    if (item.id == dict.categoryId) {
                        resHtml += `<a href="javascript:void(0)" class="px10 is-active"> <i class="f16 iconfont icon-category"></i> ${item.name} </a> \n`;
                    } else {
                        resHtml += `<a href="./category.html?categoryId=${item.id}" class="px10"> <i class="f16 iconfont icon-category"></i> ${item.name} </a> \n`;
                    }
                }
            } else {
                for (const item of data.data) {
                    resHtml += `<a href="./category.html?categoryId=${item.id}" class="px10 "> <i class="f16 iconfont icon-category"></i> ${item.name} </a> \n`;
                }
            }
            $("div.is-body:first").empty().append(resHtml);
            $("div.menu").empty().append(resHtml);
        }
    });
});

function getUrlParams(url) {
    var result = {};
    let strArr = url.split('?');
    if (strArr.length === 0) return result;

    var params = strArr[1].split('&');
    for (var item of params) {
        var dict = item.split('=');
        result[dict[0]] = dict[1];
    }
    return result;
}

