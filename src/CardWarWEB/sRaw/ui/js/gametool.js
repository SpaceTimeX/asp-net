function clone(obj) {
    var o, i, j, k;
    if (typeof (obj) != "object" || obj === null) return obj;
    if (obj instanceof (Array)) {
        o = [];
        i = 0; j = obj.length;
        for (; i < j; i++) {
            if (typeof (obj[i]) == "object" && obj[i] != null) {
                o[i] = arguments.callee(obj[i]);
            }
            else {
                o[i] = obj[i];
            }
        }
    }
    else {
        o = {};
        for (i in obj) {
            if (typeof (obj[i]) == "object" && obj[i] != null) {
                o[i] = arguments.callee(obj[i]);
            }
            else {
                o[i] = obj[i];
            }
        }
    }

    return o;
}

function GetARM(ar) {
    var t = (ar / (100 + ar)) * 100;
    var s = t.toFixed(4);
    return s + '%';
}

function GetMISS(sb) {
    var t = sb * 0.00004 * 100;
    var s = t.toFixed(4);
    return s + '%';
}

function GetBOOM(ll, zl, jz) {
    var t = jz * 0.00002 * 100 + ll * 0.000006 * 100 + zl * 0.000006 * 100;
    var s = t.toFixed(4);
    return s + '%';
}

function iGetARM(ar) {
    var t = (ar / (100 + ar));
    return t;
}

function iGetMISS(sb) {
    var t = sb * 0.00004;
    return t;
}

function iGetBOOM(ll, zl, jz) {
    var t = jz * 0.00002 + ll * 0.000006 + zl * 0.000006;
    return t;
}

//                        原始  新 新 新 新 新
function GetDifferencePro(data, a, b, c, d, e) {
    var ihp = data.Pro.HP;
    var imp = data.Pro.MP;
    var iad = data.Pro.AD;
    var iap = data.Pro.AP;
    var iar = data.Pro.AR;
    var iarm = iGetARM(iar);
    var aa, bb, cc, dd, ee;
    aa = data.Pro.LL;
    bb = data.Pro.ZL;
    cc = data.Pro.NL;
    dd = data.Pro.SB;
    ee = data.Pro.JZ;
    var imiss = iGetMISS(dd);
    var iboom = iGetBOOM(aa, bb, ee);
    var hp, mp, ad, ap, ar, arm, miss, boom;
    var hp = data.Pro.HP;
    var mp = data.Pro.MP;
    var ad = data.Pro.AD;
    var ap = data.Pro.AP;
    var ar = data.Pro.AR;
    var arm = iGetARM(ar);
    var miss = iGetMISS(d);
    var boom = iGetBOOM(a, b, e);
    ihp += 12 * aa + 18 * cc + 18 * dd;
    imp += 16 * bb + 22 * cc + 22 * dd;
    iad += 3 * aa + 3 * ee;
    iap += 5 * bb + 4 * ee;
    iar += 0.007 * cc;
    iarm = iGetARM(iar);

    hp += 12 * a + 18 * c + 18 * d;
    mp += 16 * b + 22 * c + 22 * d;
    ad += 3 * a + 3 * e;
    ap += 5 * b + 4 * e;
    ar += 0.007 * c;
    arm = iGetARM(ar);

    var dhp, dmp, dad, dap, darm, dmiss, dboom;
    dhp = hp - ihp;
    dmp = mp - imp;
    dad = ad - iad;
    dap = ap - iap;
    dar = ar - iar;
    darm = arm - iarm;
    dmiss = miss - imiss;
    dboom = boom - iboom;
    var r = {};
    r["hp"] = dhp;
    r["mp"] = dmp;
    r["ad"] = dad;
    r["ap"] = dap;
    r["ar"] = dar;
    r["arm"] = darm;
    r["miss"] = dmiss;
    r["boom"] = dboom;
    return r;
}

function GetPro(data) {
    var r = {};
    var a, b, c, d, e;
    a = data.Pro.LL;
    b = data.Pro.ZL;
    c = data.Pro.NL;
    d = data.Pro.SB;
    e = data.Pro.JZ;
    var hp, mp, ad, ap, ar, arm, miss, boom, ex, pt, level,usedpoint, nonusedpoint;
    hp = data.Pro.HP;
    mp = data.Pro.MP;
    ad = data.Pro.AD;
    ap = data.Pro.AP;
    ar = data.Pro.AR;

    ex = data.Pro.EX;
    pt = data.Pro.PT;

    usedpoint = data.Pro.UsedPoint;
    nonusedpoint = data.Pro.NonUsedPoint;

    level = data.Pro.Level;
    hp += 12 * a + 18 * c + 18 * d;
    mp += 16 * b + 22 * c + 22 * d;
    ad += 3 * a + 3 * e;
    ap += 5 * b + 4 * e;
    ar += 0.007 * c;
    arm = iGetARM(ar);
    miss = iGetMISS(d);
    boom = iGetBOOM(a, b, e);

    r["hp"] = hp;
    r["mp"] = mp;
    r["ad"] = ad;
    r["ap"] = ap;
    r["ar"] = ar;
    r["arm"] = arm;
    r["miss"] = miss;
    r["boom"] = boom;
    r["ex"] = ex;
    r["pt"] = pt;
    r["level"] = level;
    r["usedpoint"] = usedpoint;
    r["nonusedpoint"] = nonusedpoint;
    r["a"] = a;
    r["b"] = b;
    r["c"] = c;
    r["d"] = d;
    r["e"] = e;

    return r;
}