$(function () {
    // 获取路由参数
    const dict = getUrlParams(window.location.href);
    $.get(`/api/videos/${dict.videoId}`, function (data) {
        if (data.code === 0) {
            let video = data.data.video;
            $('#videoName').html(video.name);
            $('#categoryList').html(`<span class=docify-badge>${video.categoryName}</span>`);

            var info = video.info.split('\n');
            info = info.map(it => it + '<br>\n');
            $('div.collapse-content').html(info);
            $('video').attr({ src: video.path });
        }
    });
})