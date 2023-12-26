using Microsoft.VisualStudio.TestTools.UnitTesting;
using RomajiConverter.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomajiConverter.Core.Extensions;

namespace RomajiConverter.Test.Helpers
{
    [TestClass]
    public class RomajiHelperTests
    {
        public RomajiHelperTests()
        {
            RomajiHelper.Init();
        }

        /// <summary>
        /// 这里的用例不是完全正确的(来自于旧版本)，通过测试只是证明逻辑相较旧版本没有变化
        /// 改进了转换逻辑才会手动更新用例
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="hiragana"></param>
        /// <param name="romaji"></param>
        [TestMethod]
        [DataRow("色とりどりの 景色 ぽつり", "いろ とりどり の けしき ぽつり", "iro toridori no keshiki potsuri")]
        [DataRow("騒がしいのに 僕は ひとり", "さわがしい の に ぼく わ ひとり", "sawagashii no ni boku wa hitori")]
        [DataRow("大きくなった 音は ひびき", "おおきく なっ た おと わ ひびき", "ookiku na ta oto wa hibiki")]
        [DataRow("不意に手を引く 君と ともに", "ふい に て お ひく きみ と とも に", "fui ni te o hiku kimi to tomo ni")]
        [DataRow("いつか 振り返るかな そんな", "いつ か ふりかえる か な そんな", "itsu ka furikaeru ka na sonna")]
        [DataRow("今日も 期待したんだ サチュレイター", "きょう も きたい し た ん だ さちゅれいたー", "kyou mo kitai shi ta n da sachureitaa")]
        [DataRow("君に届ける 電子音 忘れてしまう前に", "きみ に とどける でんし おん わすれ て しまう まえ に", "kimi ni todokeru denshi on wasure te shimau mae ni")]
        [DataRow("おぼろげな歌も 本当は きっと 歌える", "おぼろ げ な うた も ほんとう わ きっと うたえる", "oboro ge na uta mo hontou wa kitto utaeru")]
        [DataRow("君に届かぬ 電子音 滲ませるのは 涙", "きみ に とどか ぬ でんし おん にじま せる の わ なみだ", "kimi ni todoka nu denshi on nijima seru no wa namida")]
        [DataRow("手を伸ばせないまま ずっと まだ 遠い 君の元へ", "て お のばせ ない まま ずっと まだ とおい きみ の もと え", "te o nobase nai mama zutto mada tooi kimi no moto e")]
        [DataRow("毎分毎秒 変わる色を", "まいふん まいびょう かわる いろ お", "maifun maibyou kawaru iro o")]
        [DataRow("追いかけていた 僕は 一人", "おいかけ て い た ぼく わ ひとり", "oikake te i ta boku wa hitori")]
        [DataRow("摑めない理由 やっと 見えた", "つかめ ない りゆう やっと みえ た", "tsukame nai riyuu yatto mie ta")]
        [DataRow("移り変わるのは 僕の心", "うつりかわる の わ ぼく の こころ", "utsurikawaru no wa boku no kokoro")]
        [DataRow("いつも ただ待っていた 僕に", "いつ も ただ まっ て い た ぼく に", "itsu mo tada ma te i ta boku ni")]
        [DataRow("今日は 期待したいんだ サチュレイター", "きょう わ きたい し たい ん だ さちゅれいたー", "kyou wa kitai shi tai n da sachureitaa")]
        [DataRow("君に届ける 電子音 なくしてしまう前に", "きみ に とどける でんし おん なくし て しまう まえ に", "kimi ni todokeru denshi on nakushi te shimau mae ni")]
        [DataRow("忘れてた歌も 本当は まだ 歌える", "わすれ て た うた も ほんとう わ まだ うたえる", "wasure te ta uta mo hontou wa mada utaeru")]
        [DataRow("君に届かぬ 電子音 止めているのは 僕だ", "きみ に とどか ぬ でんし おん とめ て いる の わ ぼく だ", "kimi ni todoka nu denshi on tome te iru no wa boku da")]
        [DataRow("手が届かないから ずっと まだ 遠い 君の元へ", "て が とどか ない から ずっと まだ とおい きみ の もと え", "te ga todoka nai kara zutto mada tooi kimi no moto e")]
        [DataRow("元へ...", "もと え . . .", "moto e . . .")]
        [DataRow("君に届ける 電子音 忘れてしまう前に", "きみ に とどける でんし おん わすれ て しまう まえ に", "kimi ni todokeru denshi on wasure te shimau mae ni")]
        [DataRow("おぼろげな歌も 本当は もっと 歌える", "おぼろ げ な うた も ほんとう わ もっと うたえる", "oboro ge na uta mo hontou wa motto utaeru")]
        [DataRow("君に届かぬ 電子音 滲ませたのは 僕だ", "きみ に とどか ぬ でんし おん にじま せ た の わ ぼく だ", "kimi ni todoka nu denshi on nijima se ta no wa boku da")]
        [DataRow("手を伸ばせないから ずっと まだ 遠い 君の元へ", "て お のばせ ない から ずっと まだ とおい きみ の もと え", "te o nobase nai kara zutto mada tooi kimi no moto e")]
        [DataRow("元へ...", "もと え . . .", "moto e . . .")]

        [DataRow("悴んだ心 ふるえる眼差し", "かじかん だ こころ ふるえる まなざし", "kajikan da kokoro furueru manazashi")]
        [DataRow("世界で 僕は ひとりぼっちだった", "せかい で ぼく わ ひとりぼっち だっ た", "sekai de boku wa hitoribotchi da ta")]
        [DataRow("散ることしか知らない春は", "ちる こと しか しら ない はる わ", "chiru koto shika shira nai haru wa")]
        [DataRow("毎年 冷たくあしらう", "まいとし つめたく あしらう", "maitoshi tsumetaku ashirau")]
        [DataRow("暗がりの中 一方通行に", "くらがり の なか いっぽう つうこう に", "kuragari no naka ippou tsuukou ni")]
        [DataRow("ただ ただ 言葉を書き殴って", "ただ ただ ことば お かきなぐっ て", "tada tada kotoba o kakinagu te")]
        [DataRow("期待するだけ むなしいと分かっていても", "きたい する だけ むなしい と わかっ て い て も", "kitai suru dake munashii to waka te i te mo")]
        [DataRow("救いを求め続けた", "すくい お もとめ つづけ た", "sukui o motome tsuzuke ta")]
        [DataRow("（せつなくて いとおしい）", "（ せつなく て いとおしい ）", "（ setsunaku te itooshii ）")]
        [DataRow("今ならば 分かる気がする", "いま なら ば わかる き が する", "ima nara ba wakaru ki ga suru")]
        [DataRow("（しあわせで くるおしい）", "（ しあわせ で くるおしい ）", "（ shiawase de kuruoshii ）")]
        [DataRow("あの日泣けなかった僕を", "あの ひ なけ なかっ た ぼく お", "ano hi nake naka ta boku o")]
        [DataRow("光は やさしく連れ立つよ", "ひかり わ やさしく つれだつ よ", "hikari wa yasashiku tsuredatsu yo")]
        [DataRow("雲間をぬって きらりきらり", "くもま お ぬっ て きらり きらり", "kumoma o nu te kirari kirari")]
        [DataRow("心満たしては 溢れ", "こころ みたし て わ あふれ", "kokoro mitashi te wa afure")]
        [DataRow("いつしか頬を きらりきらり", "いつ し か ほお お きらり きらり", "itsu shi ka hoo o kirari kirari")]
        [DataRow("熱く 熱く濡らしてゆく", "あつく あつく ぬらし て ゆく", "atsuku atsuku nurashi te yuku")]
        [DataRow("君の手は どうしてこんなにも温かいの?", "きみ の て わ どう し て こんな に も あたたかい の ?", "kimi no te wa dou shi te konna ni mo atatakai no ?")]
        [DataRow("ねぇお願い", "ねぇ お ねがい", "nee o negai")]
        [DataRow("どうかこのまま 離さないでいて", "どう か この まま はなさ ない で い て", "dou ka kono mama hanasa nai de i te")]
        [DataRow("縁を結んでは ほどきほどかれ", "えん お むすん で わ ほどき ほど かれ", "en o musun de wa hodoki hodo kare")]
        [DataRow("誰しもがそれを喜び 悲しみながら", "だれ しも が それ お よろこび かなしみ ながら", "dare shimo ga sore o yorokobi kanashimi nagara")]
        [DataRow("愛を数えてゆく", "あい お かぞえ て ゆく", "ai o kazoe te yuku")]
        [DataRow("鼓動を確かめるように", "こどう お たしかめる よう に", "kodou o tashikameru you ni")]
        [DataRow("（うれしくて さびしくて）", "（ うれしく て さびしく て ）", "（ ureshiku te sabishiku te ）")]
        [DataRow("今だから 分かる気がした", "いま だ から わかる き が し た", "ima da kara wakaru ki ga shi ta")]
        [DataRow("（たいせつで こわくって）", "（ たいせつ で こわく って ）", "（ taisetsu de kowaku tte ）")]
        [DataRow("あの日泣けなかった僕を", "あの ひ なけ なかっ た ぼく お", "ano hi nake naka ta boku o")]
        [DataRow("光は やさしく抱きしめた", "ひかり わ やさしく だきしめ た", "hikari wa yasashiku dakishime ta")]
        [DataRow("照らされた世界 咲き誇る大切な人", "てらさ れ た せかい さきほこる たいせつ な ひと", "terasa re ta sekai sakihokoru taisetsu na hito")]
        [DataRow("あたたかさを知った春は", "あたたか さ お しっ た はる わ", "atataka sa o shi ta haru wa")]
        [DataRow("僕のため 君のための 涙を流すよ", "ぼく の ため きみ の ため の なみだ お ながす よ", "boku no tame kimi no tame no namida o nagasu yo")]
        [DataRow("あぁ なんて眩しいんだろう", "あぁ なんて まぶしい ん だろう", "aa nante mabushii n darou")]
        [DataRow("あぁ なんて美しいんだろう...", "あぁ なんて うつくしい ん だろう . . .", "aa nante utsukushii n darou . . .")]
        [DataRow("雲間をぬって きらりきらり", "くもま お ぬっ て きらり きらり", "kumoma o nu te kirari kirari")]
        [DataRow("心満たしては 溢れ", "こころ みたし て わ あふれ", "kokoro mitashi te wa afure")]
        [DataRow("いつしか頬を きらりきらり", "いつ し か ほお お きらり きらり", "itsu shi ka hoo o kirari kirari")]
        [DataRow("熱く 熱く濡らしてゆく", "あつく あつく ぬらし て ゆく", "atsuku atsuku nurashi te yuku")]
        [DataRow("君の手は どうしてこんなにも温かいの?", "きみ の て わ どう し て こんな に も あたたかい の ?", "kimi no te wa dou shi te konna ni mo atatakai no ?")]
        [DataRow("ねぇお願い", "ねぇ お ねがい", "nee o negai")]
        [DataRow("どうかこのまま 離さないでいて", "どう か この まま はなさ ない で い て", "dou ka kono mama hanasa nai de i te")]
        [DataRow("ずっと ずっと 離さないでいて", "ずっと ずっと はなさ ない で い て", "zutto zutto hanasa nai de i te")]

        [DataRow("気付いてない　誰一人", "きづい て ない だれ ひとり", "kizui te nai dare hitori")]
        [DataRow("朱の空に　ただ冷ややかに", "しゅ の そら に ただ ひややか に", "shu no sora ni tada hiyayaka ni")]
        [DataRow("ビルの隙間に　恋する様に", "びる の すきま に こい する さま に", "biru no sukima ni koi suru sama ni")]
        [DataRow("見惚れてたいの　いなくなってしまう前に", "みとれ て たい の い なく なっ て しまう まえ に", "mitore te tai no i naku na te shimau mae ni")]
        [DataRow("一方的に愛を語らせてよ", "いっぽう てき に あい お かたら せ て よ", "ippou teki ni ai o katara se te yo")]
        [DataRow("しららかな三日月", "しら ら か な みっ か つき", "shira ra ka na mi ka tsuki")]
        [DataRow("さあ僕の手を取って", "さあ ぼく の て お とっ て", "saa boku no te o to te")]
        [DataRow("軽やかにステップを", "かろやか に すてっぷ お", "karoyaka ni suteppu o")]
        [DataRow("あと少しだけ", "あと すこし だけ", "ato sukoshi dake")]
        [DataRow("だけどやっぱり君は頬を膨らすのさ", "だ けど やっぱり きみ わ ほお お ふくらす の さ", "da kedo yappari kimi wa hoo o fukurasu no sa")]
        [DataRow("僕の気も知らずに", "ぼく の き も しら ず に", "boku no ki mo shira zu ni")]
        [DataRow("さあ僕の手を取って", "さあ ぼく の て お とっ て", "saa boku no te o to te")]
        [DataRow("鮮やかにステップを", "あざやか に すてっぷ お", "azayaka ni suteppu o")]
        [DataRow("夜が来るまで", "よる が くる まで", "yoru ga kuru made")]
        [DataRow("見つめても　愛しても", "みつめ て も あいし て も", "mitsume te mo aishi te mo")]
        [DataRow("その肌は　誰かのモノで", "その はだ わ だれ か の もの で", "sono hada wa dare ka no mono de")]
        [DataRow("目を疑う　カゴの中", "め お うたがう かご の なか", "me o utagau kago no naka")]
        [DataRow("売られてたんだ　虫唾が走る", "うら れ て た ん だ むし つば が はしる", "ura re te ta n da mushi tsuba ga hashiru")]
        [DataRow("いついつまでも　片想い", "いついつ まで も かたおもい", "itsuitsu made mo kataomoi")]
        [DataRow("分かってるのに　何故こんなに悔しいの？", "わかっ てる の に なぜ こんな に くやしい の ？", "waka teru no ni naze konna ni kuyashii no ？")]
        [DataRow("一方的に君を奪われた様", "いっぽう てき に きみ お うばわ れ た よう", "ippou teki ni kimi o ubawa re ta you")]
        [DataRow("そんなはずはないのに", "そんな はず わ ない の に", "sonna hazu wa nai no ni")]
        [DataRow("いくら呼んだって", "いくら よん だって", "ikura yon datte")]
        [DataRow("答えてはくれないでしょう？", "こたえ て わ くれ ない でしょう ？", "kotae te wa kure nai deshou ？")]
        [DataRow("誰の声でも", "だれ の こえ で も", "dare no koe de mo")]
        [DataRow("つまり　同様　僕のモノでもないのさ", "つまり どうよう ぼく の もの で も ない の さ", "tsumari douyou boku no mono de mo nai no sa")]
        [DataRow("嗚呼、粗末なアイロニー", "ああ 、 そまつ な あいろにー", "aa 、 somatsu na aironii")]
        [DataRow("だから君を思って", "だ から きみ お おもっ て", "da kara kimi o omo te")]
        [DataRow("泣く意味も無いのさ", "なく いみ も ない の さ", "naku imi mo nai no sa")]
        [DataRow("でも嫌なんだ！", "で も いや な ん だ ！", "de mo iya na n da ！")]
        [DataRow("僕のモノじゃないのに", "ぼく の もの じゃ ない の に", "boku no mono ja nai no ni")]
        [DataRow("僕のモノじゃないけど", "ぼく の もの じゃ ない けど", "boku no mono ja nai kedo")]
        [DataRow("一方的に愛を語らせてよ！", "いっぽう てき に あい お かたら せ て よ ！", "ippou teki ni ai o katara se te yo ！")]
        [DataRow("しららかな三日月", "しら ら か な みっ か つき", "shira ra ka na mi ka tsuki")]
        [DataRow("さあ僕の手を取って", "さあ ぼく の て お とっ て", "saa boku no te o to te")]
        [DataRow("軽やかにステップを", "かろやか に すてっぷ お", "karoyaka ni suteppu o")]
        [DataRow("あと少しだけ", "あと すこし だけ", "ato sukoshi dake")]

        [DataRow("あと少しだけ　僕は眠らずに", "あと すこし だけ ぼく わ ねむら ず に", "ato sukoshi dake boku wa nemura zu ni")]
        [DataRow("部屋を暗い海だとして", "へや お くらい うみ だ と し て", "heya o kurai umi da to shi te")]
        [DataRow("泳いだ　泳いだ", "およい だ およい だ", "oyoi da oyoi da")]
        [DataRow("あと少しだけ　僕は眠らずに", "あと すこし だけ ぼく わ ねむら ず に", "ato sukoshi dake boku wa nemura zu ni")]
        [DataRow("潜り込んだ布団の砂でほら", "もぐりこん だ ふとん の すな で ほら", "mogurikon da futon no suna de hora")]
        [DataRow("明日を見ないようにしていた", "あす お み ない よう に し て い た", "asu o mi nai you ni shi te i ta")]
        [DataRow("痛いのは　まだまだ慣れてないからかな", "いたい の わ まだまだ なれ て ない から か な", "itai no wa madamada nare te nai kara ka na")]
        [DataRow("僕は　砂 深く深く埋もれてしまったんだ", "ぼく わ すな ふかく ふかく うずもれ て しまっ た ん だ", "boku wa suna fukaku fukaku uzumore te shima ta n da")]
        [DataRow("あと少しだけ　僕は眠らずに", "あと すこし だけ ぼく わ ねむら ず に", "ato sukoshi dake boku wa nemura zu ni")]
        [DataRow("床を深い海の底として触った　触った", "ゆか お ふかい うみ の そこ と し て さわっ た さわっ た", "yuka o fukai umi no soko to shi te sawa ta sawa ta")]
        [DataRow("あと少しだけ　僕は眠らずに", "あと すこし だけ ぼく わ ねむら ず に", "ato sukoshi dake boku wa nemura zu ni")]
        [DataRow("脱ぎ捨てられた服がほら", "ぬぎすて られ た ふく が ほら", "nugisute rare ta fuku ga hora")]
        [DataRow("まるで抜け殻に見えたんだ", "まるで ぬけがら に みえ た ん だ", "marude nukegara ni mie ta n da")]
        [DataRow("痛いのは　まだまだ慣れてないからかな", "いたい の わ まだまだ なれ て ない から か な", "itai no wa madamada nare te nai kara ka na")]
        [DataRow("僕は　砂 深く深く埋もれてしまったんだ", "ぼく わ すな ふかく ふかく うずもれ て しまっ た ん だ", "boku wa suna fukaku fukaku uzumore te shima ta n da")]
        [DataRow("あと少しで　僕は眠るだろう", "あと すこし で ぼく わ ねむる だろう", "ato sukoshi de boku wa nemuru darou")]
        [DataRow("部屋に滑り込んできた光が", "へや に すべりこん で き た ひかり が", "heya ni suberikon de ki ta hikari ga")]
        [DataRow("まるで何かを言うようだ", "まるで なん か お いう よう だ", "marude nan ka o iu you da")]
        [DataRow("痛いのは　まだまだ慣れてないからかな", "いたい の わ まだまだ なれ て ない から か な", "itai no wa madamada nare te nai kara ka na")]
        [DataRow("僕は　砂 深く深く埋もれてしまったんだ", "ぼく わ すな ふかく ふかく うずもれ て しまっ た ん だ", "boku wa suna fukaku fukaku uzumore te shima ta n da")]
        [DataRow("痛みに　まだまだ慣れてない僕だから今は", "いたみ に まだまだ なれ て ない ぼく だ から いま わ", "itami ni madamada nare te nai boku da kara ima wa")]
        [DataRow("明日の砂　深く深く埋もれて眠るんだ", "あす の すな ふかく ふかく うずもれ て ねむる ん だ", "asu no suna fukaku fukaku uzumore te nemuru n da")]
        [DataRow("僕は砂", "ぼく わ すな", "boku wa suna")]
        [DataRow("僕は砂", "ぼく わ すな", "boku wa suna")]

        [DataRow("フェ", "ふぇ", "fe")]
        [DataRow("ファは", "ふぁ わ", "fa wa")]
        [DataRow("ティ", "てぃ", "ti")]
        [DataRow("アイロニー", "あいろにー", "aironii")]
        public void SentenceToRomajiTest(string sentence, string hiragana, string romaji)
        {
            var hiraganaList = new List<string>();
            var romajiList = new List<string>();

            foreach (var unitString in sentence.LineToUnits())
            {
                var units = RomajiHelper.SentenceToRomaji(unitString);
                hiraganaList.Add(string.Join(" ", units.Select(p => p.Hiragana)));
                romajiList.Add(string.Join(" ", units.Select(p => p.Romaji)));
            }

            var result_hiragana = string.Join(" ", hiraganaList);
            var result_romaji = string.Join(" ", romajiList);

            Assert.AreEqual(hiragana, result_hiragana);
            Assert.AreEqual(romaji, result_romaji);
        }
    }
}