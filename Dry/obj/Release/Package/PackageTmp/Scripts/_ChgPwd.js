function CheckInput(data) {
    if (data == "") {
        return { pass: false, error: "密碼不可以為空白" };
    }
    if (data.length < 8) {
        return { pass: false, error: "密碼長度必須8碼" };
    }

    const patternA = /[a-z]/;
    const patternB = /[A-Z]/;
    const patternC = /[0-9]/;
    const patternD = /[^A-Za-z0-9]/
    //let errorMSG = "不符密碼原則";
    let errorA = "", errorB = "", errorC = "";
    let regexA = new RegExp(patternA);
    let regexB = new RegExp(patternB);
    let regexC = new RegExp(patternC);
    let regexD = new RegExp(patternD);
    let passcount = 0;
    if (!regexA.test(data)) {
        errorA = "英文小寫";
    } else passcount++;
    if (!regexB.test(data)) {
        errorB = "英文大寫";
    } else passcount++;
    if (!regexC.test(data)) {
        errorC = "數字";
    } else passcount++;
    if (!regexD.test(data)) {
        errorD = "符號";
    } else passcount++;
    //if (errorA == "" && errorB == "" && errorC == "") {
    if (passcount >= 2) {
        return { pass: true, error: "" };
    } else {
        return { pass: false, error: `不符密碼原則,缺少${errorA}${errorB}${errorC}` }
    }

}