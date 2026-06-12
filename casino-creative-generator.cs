// ==========================================================================
// CASINO CREATIVE GENERATOR — Генератор креативов для казино-партнёрки
// ==========================================================================
// Публикует в Telegram разные типы контента:
//   🔥 BIG_WIN     — истории крупных выигрышей (позитив)
//   💀 BIG_LOSS    — истории проигрышей, ошибки, уроки (негатив)
//   🎁 BONUS_HUNT  — бонусы, промо, фриспины (позитив)
//   🧠 PRO_TIP     — советы, стратегии, лайфхаки (нейтральный)
//   💪 COMEBACK    — из минуса в плюс (мотивация)
//   🎰 HOT_SLOT    — горячий слот прямо сейчас (позитив)
// ==========================================================================

// ==========================================================================
// НАСТРОЙКИ
// ==========================================================================
string TELEGRAM_BOT_TOKEN  = "token";            // ТОКЕН ТВОЕГО БОТА
string TELEGRAM_CHANNEL_ID = "@tgchannel";       // ID КАНАЛА

// NVIDIA NIM API
string NVIDIA_API_KEY      = "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";
string NVIDIA_API_BASE     = "https://integrate.api.nvidia.com/v1/chat/completions";

string NVIDIA_MODEL_1      = "deepseek-ai/deepseek-v4-flash";
string NVIDIA_MODEL_2      = "mistralai/mistral-large-3-675b-instruct-2512";
string NVIDIA_MODEL_3      = "meta/llama-3.3-70b-instruct";

// Партнёрка
string PARTNER_LINK        = "http://partner.link";   // ТВОЯ ПАРТНЁРСКАЯ ССЫЛКА
string CASINO_NAME         = "КазиноХ";                // НАЗВАНИЕ ТВОЕГО КАЗИНО

string DB_PATH             = project.Directory + @"\creatives.db";
string LOG_DIR             = project.Directory + @"\logs\";
string LOG_PATH            = LOG_DIR + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
int    CAPTION_MAX_LEN     = 1024;
int    REQUEST_TIMEOUT_SEC = 150;
int    CREATIVES_PER_RUN   = 2;  // сколько креативов за запуск

// ==========================================================================
// ЛОГИРОВАНИЕ
// ==========================================================================
Action<string> Log = (msg) => {
    string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    string line = $"[{time}] {msg}";
    project.SendInfoToLog(msg);
    try { System.IO.File.AppendAllText(LOG_PATH, line + Environment.NewLine, System.Text.Encoding.UTF8); }
    catch { }
};

if (!System.IO.Directory.Exists(LOG_DIR))
    System.IO.Directory.CreateDirectory(LOG_DIR);

Log("========== CASINO CREATIVE GENERATOR ==========");

System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;

var cookieContainer = new System.Net.CookieContainer();
var httpHandler = new System.Net.Http.HttpClientHandler
{
    CookieContainer = cookieContainer,
    UseCookies = true,
    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
    AllowAutoRedirect = true
};

var httpClient = new System.Net.Http.HttpClient(httpHandler);
httpClient.Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT_SEC);
httpClient.DefaultRequestHeaders.Add("User-Agent",
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");
httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8");

// ==========================================================================
// БАЗА СЛОТОВ — для контекста в креативах
// ==========================================================================
var allSlots = new List<Dictionary<string, string>>
{
    new Dictionary<string, string> { {"name","Sweet Bonanza"}, {"provider","Pragmatic Play"}, {"rtp","96.51"}, {"vol","Высокая"}, {"maxwin","x21200"}, {"theme","сладости конфеты"} },
    new Dictionary<string, string> { {"name","Gates of Olympus"}, {"provider","Pragmatic Play"}, {"rtp","96.50"}, {"vol","Высокая"}, {"maxwin","x5000"}, {"theme","мифология Зевс"} },
    new Dictionary<string, string> { {"name","Sugar Rush"}, {"provider","Pragmatic Play"}, {"rtp","96.50"}, {"vol","Высокая"}, {"maxwin","x5000"}, {"theme","сладости"} },
    new Dictionary<string, string> { {"name","The Dog House"}, {"provider","Pragmatic Play"}, {"rtp","96.51"}, {"vol","Высокая"}, {"maxwin","x6750"}, {"theme","собаки"} },
    new Dictionary<string, string> { {"name","Big Bass Bonanza"}, {"provider","Pragmatic Play"}, {"rtp","96.71"}, {"vol","Средняя"}, {"maxwin","x2100"}, {"theme","рыбалка"} },
    new Dictionary<string, string> { {"name","Starlight Princess"}, {"provider","Pragmatic Play"}, {"rtp","96.50"}, {"vol","Высокая"}, {"maxwin","x5000"}, {"theme","аниме магия"} },
    new Dictionary<string, string> { {"name","Wild West Gold"}, {"provider","Pragmatic Play"}, {"rtp","96.51"}, {"vol","Высокая"}, {"maxwin","x18750"}, {"theme","ковбои запад"} },
    new Dictionary<string, string> { {"name","Book of Dead"}, {"provider","Play'n GO"}, {"rtp","96.21"}, {"vol","Высокая"}, {"maxwin","x5000"}, {"theme","Египет"} },
    new Dictionary<string, string> { {"name","Reactoonz"}, {"provider","Play'n GO"}, {"rtp","96.51"}, {"vol","Высокая"}, {"maxwin","x4570"}, {"theme","пришельцы"} },
    new Dictionary<string, string> { {"name","Dead or Alive 2"}, {"provider","NetEnt"}, {"rtp","96.82"}, {"vol","Очень высокая"}, {"maxwin","x111111"}, {"theme","дикий запад"} },
    new Dictionary<string, string> { {"name","Mental"}, {"provider","Nolimit City"}, {"rtp","96.08"}, {"vol","Очень высокая"}, {"maxwin","x9999"}, {"theme","хоррор"} },
    new Dictionary<string, string> { {"name","Wanted Dead or a Wild"}, {"provider","Hacksaw Gaming"}, {"rtp","96.38"}, {"vol","Очень высокая"}, {"maxwin","x30000"}, {"theme","вестерн"} },
    new Dictionary<string, string> { {"name","Money Train 3"}, {"provider","Relax Gaming"}, {"rtp","96.10"}, {"vol","Очень высокая"}, {"maxwin","x100000"}, {"theme","поезд запад"} },
    new Dictionary<string, string> { {"name","Bonanza"}, {"provider","Big Time Gaming"}, {"rtp","96.00"}, {"vol","Высокая"}, {"maxwin","x12000"}, {"theme","шахты золото"} },
    new Dictionary<string, string> { {"name","Jammin' Jars"}, {"provider","Push Gaming"}, {"rtp","96.83"}, {"vol","Высокая"}, {"maxwin","x20000"}, {"theme","фрукты диско"} },
    new Dictionary<string, string> { {"name","Razor Shark"}, {"provider","Push Gaming"}, {"rtp","96.70"}, {"vol","Высокая"}, {"maxwin","x5000"}, {"theme","акулы океан"} },
    new Dictionary<string, string> { {"name","Starburst"}, {"provider","NetEnt"}, {"rtp","96.09"}, {"vol","Низкая"}, {"maxwin","x500"}, {"theme","космос кристаллы"} },
    new Dictionary<string, string> { {"name","Gonzo's Quest"}, {"provider","NetEnt"}, {"rtp","95.97"}, {"vol","Средняя"}, {"maxwin","x2500"}, {"theme","инки золото"} },
    new Dictionary<string, string> { {"name","Fire Joker"}, {"provider","Play'n GO"}, {"rtp","96.15"}, {"vol","Средняя"}, {"maxwin","x800"}, {"theme","джокер классика"} },
    new Dictionary<string, string> { {"name","Moon Princess"}, {"provider","Play'n GO"}, {"rtp","96.50"}, {"vol","Высокая"}, {"maxwin","x5000"}, {"theme","аниме принцессы"} },
    new Dictionary<string, string> { {"name","Mega Moolah"}, {"provider","Microgaming"}, {"rtp","88.12"}, {"vol","Средняя"}, {"maxwin","x18000000"}, {"theme","сафари джекпот"} },
    new Dictionary<string, string> { {"name","Immortal Romance"}, {"provider","Microgaming"}, {"rtp","96.86"}, {"vol","Высокая"}, {"maxwin","x3600"}, {"theme","вампиры"} },
    new Dictionary<string, string> { {"name","Rise of Olympus"}, {"provider","Play'n GO"}, {"rtp","96.50"}, {"vol","Высокая"}, {"maxwin","x5000"}, {"theme","Греция боги"} },
    new Dictionary<string, string> { {"name","San Quentin"}, {"provider","Nolimit City"}, {"rtp","96.03"}, {"vol","Очень высокая"}, {"maxwin","x150000"}, {"theme","тюрьма"} },
    new Dictionary<string, string> { {"name","Tombstone RIP"}, {"provider","Nolimit City"}, {"rtp","96.08"}, {"vol","Очень высокая"}, {"maxwin","x300000"}, {"theme","вестерн"} },
    new Dictionary<string, string> { {"name","Extra Chilli"}, {"provider","Big Time Gaming"}, {"rtp","96.82"}, {"vol","Высокая"}, {"maxwin","x20000"}, {"theme","мексика специи"} },
    new Dictionary<string, string> { {"name","Vikings Go Berzerk"}, {"provider","Yggdrasil"}, {"rtp","96.10"}, {"vol","Средняя"}, {"maxwin","x4000"}, {"theme","викинги"} },
    new Dictionary<string, string> { {"name","Crazy Monkey"}, {"provider","Belatra"}, {"rtp","94.00"}, {"vol","Средняя"}, {"maxwin","x300"}, {"theme","обезьяна"} },
    new Dictionary<string, string> { {"name","Fruit Cocktail"}, {"provider","Igrosoft"}, {"rtp","94.00"}, {"vol","Средняя"}, {"maxwin","x250"}, {"theme","фрукты классика"} },
    new Dictionary<string, string> { {"name","Mahjong Ways 2"}, {"provider","PG Soft"}, {"rtp","96.95"}, {"vol","Средняя"}, {"maxwin","x25000"}, {"theme","маджонг"} },
};

// ==========================================================================
// ТИПЫ КОНТЕНТА — веса определяют частоту появления
// ==========================================================================
var contentTypes = new List<Dictionary<string, string>>
{
    new Dictionary<string, string> { {"type","BIG_WIN"}, {"emoji","🔥"}, {"weight","35"}, {"label","Крупный выигрыш"} },
    new Dictionary<string, string> { {"type","BIG_LOSS"}, {"emoji","💀"}, {"weight","20"}, {"label","Проигрыш / ошибки"} },
    new Dictionary<string, string> { {"type","BONUS_HUNT"}, {"emoji","🎁"}, {"weight","15"}, {"label","Бонус / промо"} },
    new Dictionary<string, string> { {"type","PRO_TIP"}, {"emoji","🧠"}, {"weight","15"}, {"label","Совет / стратегия"} },
    new Dictionary<string, string> { {"type","COMEBACK"}, {"emoji","💪"}, {"weight","10"}, {"label","Камбэк из минуса"} },
    new Dictionary<string, string> { {"type","HOT_SLOT"}, {"emoji","🎰"}, {"weight","5"}, {"label","Горячий слот"} },
};

// ==========================================================================
// NVIDIA NIM API
// ==========================================================================
Func<string, string, string> CallNvidia = (prompt, model) =>
{
    if (prompt.Length > 30000)
        prompt = prompt.Substring(0, 30000);

    var body = Newtonsoft.Json.JsonConvert.SerializeObject(new
    {
        model = model,
        messages = new[] { new { role = "user", content = prompt } },
        max_tokens = 4096,
        temperature = 0.9,
        top_p = 0.95
    });

    try
    {
        var req = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, NVIDIA_API_BASE);
        req.Content = new System.Net.Http.StringContent(body, Encoding.UTF8, "application/json");
        req.Headers.Add("Authorization", $"Bearer {NVIDIA_API_KEY}");
        req.Headers.Add("Accept", "application/json");

        var resp = httpClient.SendAsync(req).Result;
        string respBody = resp.Content.ReadAsStringAsync().Result;

        if (!resp.IsSuccessStatusCode)
        {
            Log($"  [NVIDIA/{model}] HTTP {(int)resp.StatusCode}: {respBody.Substring(0, Math.Min(300, respBody.Length))}");
            return "";
        }

        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(respBody);
        if (obj == null || obj.choices == null || obj.choices.Count == 0)
            return "";

        return ((string)obj.choices[0].message.content).Trim();
    }
    catch (Exception ex)
    {
        Log($"  [NVIDIA/{model}] Исключение: {ex.Message}");
        return "";
    }
};

Func<string, string> CallAI = (prompt) =>
{
    string[] models = { NVIDIA_MODEL_1, NVIDIA_MODEL_2, NVIDIA_MODEL_3 };
    string[] names  = { "DeepSeek-V4-Flash", "Mistral-Large-3", "Llama-3.3-70B" };

    for (int i = 0; i < models.Length; i++)
    {
        Log($"    AI: пробуем {names[i]}...");
        string result = CallNvidia(prompt, models[i]);

        if (!string.IsNullOrWhiteSpace(result))
        {
            Log($"    AI: ответ от {names[i]} ({result.Length} символов)");
            return result;
        }

        if (i < models.Length - 1)
            Log($"    AI: {names[i]} не ответил, пробуем {names[i + 1]}...");
    }

    Log("    AI: ВСЕ модели не ответили!");
    return "";
};

// ==========================================================================
// Очистка ответа AI
// ==========================================================================
Func<string, string> CleanResponse = (text) =>
{
    string result = text.Trim();
    if ((result.StartsWith("\"") && result.EndsWith("\"")) ||
        (result.StartsWith("'") && result.EndsWith("'")) ||
        (result.StartsWith("«") && result.EndsWith("»")))
    {
        result = result.Substring(1, result.Length - 2).Trim();
    }
    string[] servicePhrases = new[] {
        "Вот перевод:", "Перевод:", "Вот обзор:", "Обзор слота:",
        "Вот текст:", "Вот пост:", "Вот креатив:", "---", "***"
    };
    foreach (var phrase in servicePhrases)
    {
        if (result.StartsWith(phrase))
            result = result.Substring(phrase.Length).Trim();
    }
    result = System.Text.RegularExpressions.Regex.Replace(result, @"^```[\w]*\s*", "");
    if (result.EndsWith("```")) result = result.Substring(0, result.Length - 3).Trim();
    return result.Trim();
};

// ==========================================================================
// ВЫБОР ТИПА КОНТЕНТА ПО ВЕСАМ
// ==========================================================================
Func<string> PickContentType = () =>
{
    var rand = new Random(DateTime.Now.Millisecond);
    int totalWeight = 0;
    foreach (var ct in contentTypes)
        totalWeight += int.Parse(ct["weight"]);

    int pick = rand.Next(totalWeight);
    int cumulative = 0;
    foreach (var ct in contentTypes)
    {
        cumulative += int.Parse(ct["weight"]);
        if (pick < cumulative)
            return ct["type"];
    }
    return "BIG_WIN";
};

// ==========================================================================
// ПРОМПТЫ ДЛЯ КАЖДОГО ТИПА КОНТЕНТА
// ==========================================================================
Func<string, Dictionary<string,string>, string> BuildPrompt = (type, slot) =>
{
    string slotName = slot["name"];
    string provider = slot["provider"];
    string maxwin   = slot["maxwin"];
    string vol      = slot["vol"];
    string theme    = slot["theme"];

    var rand = new Random(DateTime.Now.Millisecond + slotName.Length);
    int bet = new[] {50, 100, 200, 250, 500, 1000, 2000, 5000}[rand.Next(8)];
    string[] currencies = {"руб", "$", "евро"};
    string cur = currencies[rand.Next(3)];

    switch (type)
    {
        case "BIG_WIN":
            return
                $"Напиши пост для Telegram-канала про казино. Формат: КРЕАТИВ С ВЫИГРЫШЕМ.\n\n" +
                $"Контекст: слот \"{slotName}\" от {provider}, максимальный выигрыш {maxwin}, волатильность {vol}.\n" +
                $"Ставка была {bet} {cur}.\n\n" +
                $"СТИЛЬ: как будто стример или игрок рассказывает реальную историю крупного выигрыша.\n\n" +
                $"Требования:\n" +
                $"1. Начни с ДРАМАТИЧНОГО заголовка с эмодзи (🔥💎⚡💰🚀)\n" +
                $"2. Опиши эмоции в момент когда выпал крупный выигрыш — сердце колотится, руки трясутся\n" +
                $"3. Упомяни конкретный слот и ставку\n" +
                $"4. Добавь цифры — сумма выигрыша, множитель\n" +
                $"5. Концовка — короткий призыв попробовать\n" +
                $"6. Тон: живой, эмоциональный, как друг рассказывает\n" +
                $"7. Длина: 500-800 символов\n" +
                $"8. Верни ТОЛЬКО текст поста\n" +
                $"9. НЕ добавляй хештеги и ссылки";

        case "BIG_LOSS":
            return
                $"Напиши пост для Telegram-канала про казино. Формат: ИСТОРИЯ ПРОИГРЫША + УРОК.\n\n" +
                $"Контекст: слот \"{slotName}\" от {provider}, волатильность {vol}.\n\n" +
                $"СТИЛЬ: честная история проигрыша, как игрок делал ошибки и потерял деньги.\n\n" +
                $"Требования:\n" +
                $"1. Начни с ЖЁСТКОГО заголовка (💀💔⚠️😔)\n" +
                $"2. Опиши ситуацию — как начал играть и увлёкся\n" +
                $"3. Покажи конкретные цифры — сколько проиграл\n" +
                $"4. ВАЖНО: дай УРОК — что делать правильно (контроль, лимиты, банкролл-менеджмент)\n" +
                $"5. Не читай мораль — просто честный рассказ с выводом\n" +
                $"6. Концовка — позитивная: «играй с головой и будет результат»\n" +
                $"7. Тон: честный, без прикрас, как у друга который обожёгся\n" +
                $"8. Длина: 500-800 символов\n" +
                $"9. Верни ТОЛЬКО текст поста\n" +
                $"10. НЕ добавляй хештеги и ссылки";

        case "BONUS_HUNT":
            return
                $"Напиши пост для Telegram-канала про казино. Формат: НАШЁЛ БОНУС.\n\n" +
                $"Контекст: слот \"{slotName}\" от {provider}, фриспины и бонусы.\n\n" +
                $"СТИЛЬ: азартный охотник за бонусами делится находкой.\n\n" +
                $"Требования:\n" +
                $"1. Начни с цепляющего заголовка (🎁🔥💰🎰)\n" +
                $"2. Расскажи про конкретный бонус или промо\n" +
                $"3. Объясни почему это выгодно — фриспины, множители, вейджер\n" +
                $"4. Добавь совет как максимально использовать бонус\n" +
                $"5. Концовка — призыв забрать бонус пока есть\n" +
                $"6. Тон: деловой азарта, эксперт который знает где бесплатные деньги\n" +
                $"7. Длина: 400-600 символов\n" +
                $"8. Верни ТОЛЬКО текст поста\n" +
                $"9. НЕ добавляй хештеги и ссылки";

        case "PRO_TIP":
            return
                $"Напиши пост для Telegram-канала про казино. Формат: ПРО СОВЕТ / СТРАТЕГИЯ.\n\n" +
                $"Контекст: слот \"{slotName}\" от {provider}, RTP {slot["rtp"]}%, волатильность {vol}, макс выигрыш {maxwin}.\n\n" +
                $"СТИЛЬ: опытный игрок делится реальным советом.\n\n" +
                $"Требования:\n" +
                $"1. Начни с интригующего заголовка (🧠💡⚡🎯)\n" +
                $"2. Дай конкретный рабочий совет — банкролл-менеджмент, выбор слота, тайминг, стратегия\n" +
                $"3. Объясни ПОЧЕМУ это работает\n" +
                $"4. Приведи пример с цифрами\n" +
                $"5. Предупреди о рисках — казино это развлечение а не заработок\n" +
                $"6. Тон: экспертный но простой, без зауми\n" +
                $"7. Длина: 500-700 символов\n" +
                $"8. Верни ТОЛЬКО текст поста\n" +
                $"9. НЕ добавляй хештеги и ссылки";

        case "COMEBACK":
            return
                $"Напиши пост для Telegram-канала про казино. Формат: КАМБЭК — ИЗ МИНУСА В ПЛЮС.\n\n" +
                $"Контекст: слот \"{slotName}\" от {provider}, макс выигрыш {maxwin}.\n\n" +
                $"СТИЛЬ: вдохновляющая история как игрок отыгрался после минуса.\n\n" +
                $"Требования:\n" +
                $"1. Начни с мощного заголовка (💪🔥⚡🏆)\n" +
                $"2. Начни с ситуации минуса — сколько был в долгу\n" +
                $"3. Опиши момент перелома — когда начало везти\n" +
                $"4. Покажи финальный результат — цифры выигрыша\n" +
                $"5. ВАЖНО: подчеркни что это не система а удача и правильный подход\n" +
                $"6. Урок: не гнись за потерями, делай перерывы, возвращайся с холодной головой\n" +
                $"7. Тон: мотивационный но честный, без обмана\n" +
                $"8. Длина: 500-800 символов\n" +
                $"9. Верни ТОЛЬКО текст поста\n" +
                $"10. НЕ добавляй хештеги и ссылки";

        case "HOT_SLOT":
            return
                $"Напиши пост для Telegram-канала про казино. Формат: ГОРЯЧИЙ СЛОТ ПРЯМО СЕЙЧАС.\n\n" +
                $"Контекст: слот \"{slotName}\" от {provider}, RTP {slot["rtp"]}%, волатильность {vol}, макс выигрыш {maxwin}.\n\n" +
                $"СТИЛЬ: срочная рекомендация — этот слот сейчас раздаёт.\n\n" +
                $"Требования:\n" +
                $"1. Заголовок: «ГОРЯЧИЙ СЛОТ» с эмодзи (🎰🔥⏰)\n" +
                $"2. Объясни почему именно сейчас стоит попробовать этот слот\n" +
                $"3. Перечисли главные фичи — бонуски, фриспины, множители\n" +
                $"4. Упомяни RTP и волатильность простым языком\n" +
                $"5. Концовка: «попробуй пока раздаёт»\n" +
                $"6. Тон: срочный, как инсайдерская инфа\n" +
                $"7. Длина: 400-600 символов\n" +
                $"8. Верни ТОЛЬКО текст поста\n" +
                $"9. НЕ добавляй хештеги и ссылки";

        default:
            return "Напиши короткий мотивационный пост про казино для Telegram. Верни только текст.";
    }
};

// ==========================================================================
// ПОИСК КАРТИНКИ
// ==========================================================================
Func<string, string, string> SearchImage = (query, type) =>
{
    try
    {
        string searchQ = "";
        switch (type)
        {
            case "BIG_WIN":
            case "COMEBACK":
                searchQ = $"{query} big win screenshot";
                break;
            case "BIG_LOSS":
                searchQ = $"{query} slot game";
                break;
            case "BONUS_HUNT":
                searchQ = $"{query} bonus free spins";
                break;
            case "PRO_TIP":
                searchQ = $"{query} slot strategy";
                break;
            default:
                searchQ = $"{query} slot game";
                break;
        }

        string encodedQ = Uri.EscapeDataString(searchQ);
        string searchUrl = $"https://html.duckduckgo.com/html/?q={encodedQ}";

        try { var _ = httpClient.GetStringAsync("https://duckduckgo.com/").Result; }
        catch { }
        System.Threading.Thread.Sleep(1500);

        var searchReq = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, searchUrl);
        searchReq.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
        var searchResp = httpClient.SendAsync(searchReq).Result;
        string searchHtml = searchResp.Content.ReadAsStringAsync().Result;

        var searchDoc = new HtmlAgilityPack.HtmlDocument();
        searchDoc.LoadHtml(searchHtml);

        var imgs = searchDoc.DocumentNode.SelectNodes("//img[contains(@src,'jpg') or contains(@src,'png') or contains(@src,'webp')]");
        if (imgs != null && imgs.Count > 0)
        {
            foreach (var img in imgs)
            {
                string src = img.GetAttributeValue("src", "");
                if (!string.IsNullOrEmpty(src) && src.StartsWith("http"))
                {
                    return src;
                }
            }
        }

        var metaImgs = searchDoc.DocumentNode.SelectNodes("//meta[@property='og:image' or @name='twitter:image']");
        if (metaImgs != null)
        {
            foreach (var meta in metaImgs)
            {
                string content = meta.GetAttributeValue("content", "");
                if (!string.IsNullOrEmpty(content) && content.StartsWith("http"))
                {
                    return content;
                }
            }
        }
    }
    catch (Exception ex)
    {
        Log($"    Поиск картинки: {ex.Message}");
    }

    return "";
};

// ==========================================================================
// СОЗДАНИЕ ЦВЕТНОЙ ЗАГЛУШКИ ЧЕРЕЗ PLACEHOLD.CO
// ==========================================================================
Func<string, string, string> GetPlaceholderImage = (type, slotName) =>
{
    string bg = "1a1a2e";
    string fg = "ffffff";
    string accent = "";

    switch (type)
    {
        case "BIG_WIN":
            bg = "0d1b2a"; fg = "ffd700";
            accent = "BIG+WIN";
            break;
        case "BIG_LOSS":
            bg = "1a0000"; fg = "ff4444";
            accent = "LOSS";
            break;
        case "BONUS_HUNT":
            bg = "0a1628"; fg = "00ff88";
            accent = "BONUS";
            break;
        case "PRO_TIP":
            bg = "1a1a2e"; fg = "00d4ff";
            accent = "TIP";
            break;
        case "COMEBACK":
            bg = "1a0a2e"; fg = "ff6600";
            accent = "COMEBACK";
            break;
        case "HOT_SLOT":
            bg = "2a0a0a"; fg = "ff3366";
            accent = "HOT";
            break;
    }

    string text = Uri.EscapeDataString($"{accent}+{slotName}");
    return $"https://placehold.co/1080x1080/{bg}/{fg}?text={text}";
};

// ==========================================================================
// ОТПРАВКА В TELEGRAM
// ==========================================================================
Func<string, string, string, string, int> SendToTelegram = (caption, imageUrl, type, slotName) =>
{
    bool sent = false;
    int msgId = 0;

    // Попытка 1: sendPhoto с реальной картинкой
    if (!string.IsNullOrEmpty(imageUrl))
    {
        try
        {
            string tgUrl = $"https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendPhoto";
            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                chat_id    = TELEGRAM_CHANNEL_ID,
                photo      = imageUrl,
                caption    = caption,
                parse_mode = "HTML"
            });

            var resp = httpClient.PostAsync(tgUrl,
                new System.Net.Http.StringContent(payload, Encoding.UTF8, "application/json")
            ).Result;
            string respBody = resp.Content.ReadAsStringAsync().Result;

            if (resp.IsSuccessStatusCode)
            {
                var tgObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(respBody);
                msgId = Convert.ToInt32(tgObj.result.message_id);
                sent = true;
                Log($"    sendPhoto: msg_id={msgId}");
            }
            else
            {
                Log($"    sendPhoto ошибка: {respBody.Substring(0, Math.Min(200, respBody.Length))}");
            }
        }
        catch (Exception ex)
        {
            Log($"    sendPhoto исключение: {ex.Message}");
        }
    }

    // Попытка 2: sendPhoto с заглушкой
    if (!sent)
    {
        string placeholder = GetPlaceholderImage(type, slotName);
        try
        {
            string tgUrl = $"https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendPhoto";
            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                chat_id    = TELEGRAM_CHANNEL_ID,
                photo      = placeholder,
                caption    = caption,
                parse_mode = "HTML"
            });

            var resp = httpClient.PostAsync(tgUrl,
                new System.Net.Http.StringContent(payload, Encoding.UTF8, "application/json")
            ).Result;
            string respBody = resp.Content.ReadAsStringAsync().Result;

            if (resp.IsSuccessStatusCode)
            {
                var tgObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(respBody);
                msgId = Convert.ToInt32(tgObj.result.message_id);
                sent = true;
                Log($"    sendPhoto (заглушка): msg_id={msgId}");
            }
            else
            {
                Log($"    sendPhoto заглушка ошибка: {respBody.Substring(0, Math.Min(200, respBody.Length))}");
            }
        }
        catch (Exception ex)
        {
            Log($"    sendPhoto заглушка исключение: {ex.Message}");
        }
    }

    // Попытка 3: sendMessage без картинки
    if (!sent)
    {
        try
        {
            string tgUrl = $"https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendMessage";
            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                chat_id    = TELEGRAM_CHANNEL_ID,
                text       = caption,
                parse_mode = "HTML"
            });

            var resp = httpClient.PostAsync(tgUrl,
                new System.Net.Http.StringContent(payload, Encoding.UTF8, "application/json")
            ).Result;
            string respBody = resp.Content.ReadAsStringAsync().Result;

            if (resp.IsSuccessStatusCode)
            {
                var tgObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(respBody);
                msgId = Convert.ToInt32(tgObj.result.message_id);
                sent = true;
                Log($"    sendMessage: msg_id={msgId}");
            }
            else
            {
                Log($"    sendMessage ошибка: {respBody.Substring(0, Math.Min(200, respBody.Length))}");
            }
        }
        catch (Exception ex)
        {
            Log($"    sendMessage исключение: {ex.Message}");
        }
    }

    return msgId;
};

// ==========================================================================
// SQLite — создание таблиц
// ==========================================================================
using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={DB_PATH};Version=3;"))
{
    conn.Open();

    using (var cmd = new System.Data.SQLite.SQLiteCommand(@"
        CREATE TABLE IF NOT EXISTS posted_creatives (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            content_type TEXT,
            slot_name TEXT,
            post_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            telegram_msg_id INTEGER
        );", conn))
    {
        cmd.ExecuteNonQuery();
    }
    Log("База данных готова");

    // ==========================================================================
    // ГЛАВНЫЙ ЦИКЛ — генерация креативов
    // ==========================================================================
    int publishedCount = 0;
    var rand = new Random(DateTime.Now.Millisecond);

    for (int i = 0; i < CREATIVES_PER_RUN; i++)
    {
        // Выбираем тип контента
        string type = PickContentType();
        var typeInfo = contentTypes.Find(ct => ct["type"] == type);
        string typeEmoji = typeInfo["emoji"];
        string typeLabel = typeInfo["label"];

        Log($"\n--- Креатив {i+1}/{CREATIVES_PER_RUN}: {typeEmoji} {typeLabel} ---");

        // Выбираем случайный слот
        var slot = allSlots[rand.Next(allSlots.Count)];
        string slotName = slot["name"];
        string provider = slot["provider"];

        Log($"    Слот: {slotName} ({provider})");

        // Проверяем не слишком ли много постов этого типа подряд
        int recentSameType = 0;
        try
        {
            using (var checkCmd = new System.Data.SQLite.SQLiteCommand(
                "SELECT COUNT(*) FROM posted_creatives WHERE content_type=@type AND post_date > datetime('now', '-1 day')", conn))
            {
                checkCmd.Parameters.AddWithValue("@type", type);
                recentSameType = Convert.ToInt32(checkCmd.ExecuteScalar());
            }
        }
        catch { }

        if (recentSameType >= 3)
        {
            Log($"    Тип {type} уже {recentSameType} за день, переключаем...");
            var otherTypes = contentTypes.Where(ct => ct["type"] != type).ToList();
            type = otherTypes[rand.Next(otherTypes.Count)]["type"];
            typeInfo = contentTypes.Find(ct => ct["type"] == type);
            typeEmoji = typeInfo["emoji"];
            typeLabel = typeInfo["label"];
            Log($"    Новый тип: {typeEmoji} {typeLabel}");
        }

        // Генерируем промпт
        string prompt = BuildPrompt(type, slot);
        Log("    Генерируем текст через AI...");

        string postText = CallAI(prompt);

        if (string.IsNullOrWhiteSpace(postText))
        {
            Log("    AI не сгенерировал текст, пропускаем");
            continue;
        }

        postText = CleanResponse(postText);
        Log($"    Текст: {postText.Length} символов");

        // Формируем финальный caption с партнёрской ссылкой
        string ctaLine = "";
        switch (type)
        {
            case "BIG_WIN":
            case "COMEBACK":
            case "HOT_SLOT":
                ctaLine = $"\n\n🎰 Попробовать в {CASINO_NAME} → {PARTNER_LINK}";
                break;
            case "BIG_LOSS":
                ctaLine = $"\n\n🎰 Играй с умом в {CASINO_NAME} → {PARTNER_LINK}";
                break;
            case "BONUS_HUNT":
                ctaLine = $"\n\n🎁 Забрать бонус в {CASINO_NAME} → {PARTNER_LINK}";
                break;
            case "PRO_TIP":
                ctaLine = $"\n\n🎰 Примени совет в {CASINO_NAME} → {PARTNER_LINK}";
                break;
            default:
                ctaLine = $"\n\n🎮 Играть в {CASINO_NAME} → {PARTNER_LINK}";
                break;
        }

        // Хештеги
        string[] baseTags = {"казино", "слоты"};
        string[] typeTags;
        switch (type)
        {
            case "BIG_WIN":    typeTags = new[] {"выигрыш", "джекпот"}; break;
            case "BIG_LOSS":   typeTags = new[] {"проигрыш", "ошибки"}; break;
            case "BONUS_HUNT": typeTags = new[] {"бонус", "фриспины"}; break;
            case "PRO_TIP":    typeTags = new[] {"совет", "стратегия"}; break;
            case "COMEBACK":   typeTags = new[] {"камбэк", "отыгрыш"}; break;
            case "HOT_SLOT":   typeTags = new[] {"горячийслот", slotName.Replace(" ","").Replace("'","")}; break;
            default:           typeTags = new[] {"казино"}; break;
        }
        var allTags = baseTags.Concat(typeTags).Take(4).ToList();
        string tagsText = string.Join(" ", allTags.Select(t => $"#{t}"));

        string caption = $"{postText}{ctaLine}\n\n{tagsText}";

        if (caption.Length > CAPTION_MAX_LEN)
        {
            int maxText = CAPTION_MAX_LEN - ctaLine.Length - tagsText.Length - 20;
            if (maxText < 100) maxText = 100;
            if (postText.Length > maxText)
                postText = postText.Substring(0, maxText) + "...";
            caption = $"{postText}{ctaLine}\n\n{tagsText}";
        }

        Log($"    Caption: {caption.Length} символов");

        // Ищем картинку
        Log("    Ищем картинку...");
        string imageQuery = $"{slotName} {provider}";
        string imageUrl = SearchImage(imageQuery, type);

        if (!string.IsNullOrEmpty(imageUrl))
            Log($"    Картинка найдена: {imageUrl.Substring(0, Math.Min(80, imageUrl.Length))}...");
        else
            Log("    Картинка не найдена, будет заглушка");

        // Отправляем в Telegram
        Log("    Отправляем в Telegram...");
        int msgId = SendToTelegram(caption, imageUrl, type, slotName);

        if (msgId > 0)
        {
            using (var insertCmd = new System.Data.SQLite.SQLiteCommand(
                "INSERT INTO posted_creatives (content_type, slot_name, telegram_msg_id) VALUES (@type, @name, @mid)", conn))
            {
                insertCmd.Parameters.AddWithValue("@type", type);
                insertCmd.Parameters.AddWithValue("@name", slotName);
                insertCmd.Parameters.AddWithValue("@mid", msgId);
                insertCmd.ExecuteNonQuery();
            }
            publishedCount++;
            Log($"    Сохранено в БД");
        }
        else
        {
            Log("    НЕ отправлено");
        }

        // Пауза между креативами
        if (i < CREATIVES_PER_RUN - 1)
        {
            int pause = 3000 + rand.Next(4000);
            Log($"    Пауза {pause/1000} сек...");
            System.Threading.Thread.Sleep(pause);
        }
    }

    Log($"\n========== ИТОГО: опубликовано {publishedCount} из {CREATIVES_PER_RUN} креативов ==========");

    // ==========================================================================
    // СТАТИСТИКА
    // ==========================================================================
    try
    {
        using (var statCmd = new System.Data.SQLite.SQLiteCommand(
            "SELECT content_type, COUNT(*) as cnt FROM posted_creatives GROUP BY content_type ORDER BY cnt DESC", conn))
        using (var reader = statCmd.ExecuteReader())
        {
            Log("\n--- Статистика по типам ---");
            while (reader.Read())
            {
                string ct = reader.GetString(0);
                int cnt = reader.GetInt32(1);
                var ctInfo = contentTypes.Find(c => c["type"] == ct);
                string ctEmoji = ctInfo != null ? ctInfo["emoji"] : "?";
                Log($"  {ctEmoji} {ct}: {cnt} постов");
            }
        }
    }
    catch { }
}

httpClient.Dispose();
httpHandler.Dispose();

return "OK";
