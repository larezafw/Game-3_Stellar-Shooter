using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyword 
{
    public static Color DARK_YELLOW = new Color(0.6487132f, 0.6792453f, 0.2178711f);

    public static string BASE_MANAGER = "base.manager";

    public static string iNGAME_SCENE = "ingame.scene";
    public static string MAINMENU_SCENE = "mainmenu.scene";
    public static string HIGHEST_SCORE_PREF = "highest.score";
    public static string MAX_COMBO_PREF = "highest.combo";

    public static string SKIP_TUTORIAL = "skip.tutorial";
    public static string SKIP_TUTORIAL_ICON = "skip.tutorial.icon";
    public static string CLIP_CODE = "clip code";
    public static string TUTORIAL_1_BODY = "<color=#00FFFF><b>Klik dan drag</color></b> Mouse pada bagian dalam persegi untuk membuat anak panah. " +
        "Membuat 2 atau lebih anak panah akan membuatnya bergerak";
    public static string TUTORIAL_2_BODY = "<size=17>Jika anak panah mengenai <b><color=#FF9DFF>segitiga</color></b> diluar kotak, maka <color=orange>mode combo</color> akan diaktifkan. " +
        "Menambahkan <color=green>0.75 detik</color> sisa waktu, dan memperoleh <color=yellow>score</color> sesuai banyaknya combo. " +
        "Banyaknya combo juga akan mempengaruhi <color=#00FFFF>kecepatan pembuatan</color> anak panah.";
}
