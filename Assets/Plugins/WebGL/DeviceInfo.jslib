mergeInto(LibraryManager.library, {
    GetDeviceType: function () {
        //window.alert("TEST_GetDeviceType");
        const userAgent = navigator.userAgent.toLowerCase();
        //window.alert(userAgent);
        console.log("TEST_GetDeviceType：" + userAgent);
        if (/mobile|android|iphone|ipad|ipod|windows phone/i.test(userAgent)) {
            return 0;
        } else if (/tablet|ipad/i.test(userAgent)) {
            return 1;
        } else {
            return 2;
        }
    }
});