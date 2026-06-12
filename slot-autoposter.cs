// ==========================================================================
// НАСТРОЙКИ
// ==========================================================================
string TELEGRAM_BOT_TOKEN  = "token";
string TELEGRAM_CHANNEL_ID = "@tgchannel";

// NVIDIA NIM API
string NVIDIA_API_KEY      = "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";
string NVIDIA_API_BASE     = "https://integrate.api.nvidia.com/v1/chat/completions";

string NVIDIA_MODEL_1      = "deepseek-ai/deepseek-v4-flash";
string NVIDIA_MODEL_2      = "mistralai/mistral-large-3-675b-instruct-2512";
string NVIDIA_MODEL_3      = "meta/llama-3.3-70b-instruct";

// Партнёрка
string PARTNER_LINK        = "http://partner.link";   // ТВОЯ ПАРТНЁРСКАЯ ССЫЛКА
string CASINO_NAME         = "КазиноХ";                // НАЗВАНИЕ ТВОЕГО КАЗИНО

string DB_PATH             = project.Directory + @"\articles.db";
string LOG_DIR             = project.Directory + @"\logs\";
string LOG_PATH            = LOG_DIR + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
int    CAPTION_MAX_LEN     = 1024;
int    REQUEST_TIMEOUT_SEC = 150;
int    SLOTS_PER_RUN       = 3;  // сколько слотов публиковать за запуск

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

Log("========== СТАРТ СЛОТ-АВТОПОСТЕРА ==========");

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
// БАЗА СЛОТОВ — 60+ популярных слотов
// ==========================================================================
var allSlots = new List<Dictionary<string, string>>
{
    // Pragmatic Play
    new Dictionary<string, string> { {"name","Sweet Bonanza"}, {"provider","Pragmatic Play"}, {"rtp","96.51"}, {"vol","Высокая"}, {"features","Каскадные спины, фриспины с множителями, бомбочки-конфеты"}, {"theme","сладости конфеты фрукты"} },
    new Dictionary<string, string> { {"name","Gates of Olympus"}, {"provider","Pragmatic Play"}, {"rtp","96.50"}, {"vol","Высокая"}, {"features","Каскадные спины, множители Зевса до x500, фриспины"}, {"theme","мифология Зевс олимп"} },
    new Dictionary<string, string> { {"name","Sugar Rush"}, {"provider","Pragmatic Play"}, {"rtp","96.50"}, {"vol","Высокая"}, {"features","Каскадные спины, множители позиций, фриспины"}, {"theme","сладости конфеты"} },
    new Dictionary<string, string> { {"name","Book of Golden Sands"}, {"provider","Pragmatic Play"}, {"rtp","96.01"}, {"vol","Высокая"}, {"features","Расширяющиеся символы, фриспины, бонус-бай"}, {"theme","Египет пески"} },
    new Dictionary<string, string> { {"name","The Dog House"}, {"provider","Pragmatic Play"}, {"rtp","96.51"}, {"vol","Высокая"}, {"features","Липкие вайлды, фриспины, случайные множители"}, {"theme","собаки милые"} },
    new Dictionary<string, string> { {"name","Big Bass Bonanza"}, {"provider","Pragmatic Play"}, {"rtp","96.71"}, {"vol","Средняя"}, {"features","Сбор рыб, фриспины с прогрессом, множитель до x10"}, {"theme","рыбалка рыба"} },
    new Dictionary<string, string> { {"name","Buffalo King"}, {"provider","Pragmatic Play"}, {"rtp","96.06"}, {"vol","Высокая"}, {"features","Фриспины с множителями до x324, каскады"}, {"theme","природа бизоны"} },
    new Dictionary<string, string> { {"name","Starlight Princess"}, {"provider","Pragmatic Play"}, {"rtp","96.50"}, {"vol","Высокая"}, {"features","Каскадные спины, множители принцессы, фриспины"}, {"theme","аниме принцесса магия"} },
    new Dictionary<string, string> { {"name","Wild West Gold"}, {"provider","Pragmatic Play"}, {"rtp","96.51"}, {"vol","Высокая"}, {"features","Липкие вайлды, фриспины, множители x2-x5"}, {"theme","дикий запад ковбои"} },
    new Dictionary<string, string> { {"name","John Hunter and the Book of Tut"}, {"provider","Pragmatic Play"}, {"rtp","96.50"}, {"vol","Высокая"}, {"features","Расширяющийся символ, фриспины, бонус-бай"}, {"theme","приключения Египет"} },

    // NetEnt
    new Dictionary<string, string> { {"name","Starburst"}, {"provider","NetEnt"}, {"rtp","96.09"}, {"vol","Низкая"}, {"features","Расширяющиеся вайлды, ре-спины, двусторонние линии"}, {"theme","космос звёзды кристаллы"} },
    new Dictionary<string, string> { {"name","Gonzo's Quest"}, {"provider","NetEnt"}, {"rtp","95.97"}, {"vol","Средняя"}, {"features","Каскадные спины, множители x1-x5, фриспины"}, {"theme","приключения инки золото"} },
    new Dictionary<string, string> { {"name","Dead or Alive 2"}, {"provider","NetEnt"}, {"rtp","96.82"}, {"vol","Очень высокая"}, {"features","3 типа фриспинов, липкие вайлды, множители до x5"}, {"theme","дикий запад"} },

    // Play'n GO
    new Dictionary<string, string> { {"name","Book of Dead"}, {"provider","Play'n GO"}, {"rtp","96.21"}, {"vol","Высокая"}, {"features","Расширяющийся символ, фриспины, гamble"}, {"theme","Египет Ричард Уайлд"} },
    new Dictionary<string, string> { {"name","Reactoonz"}, {"provider","Play'n GO"}, {"rtp","96.51"}, {"vol","Высокая"}, {"features","Каскадные спины, квантовые бонусы, варп-призраки"}, {"theme","пришельцы инопланетяне"} },
    new Dictionary<string, string> { {"name","Fire Joker"}, {"provider","Play'n GO"}, {"rtp","96.15"}, {"vol","Средняя"}, {"features","Ре-спины, колесо множителей до x10"}, {"theme","фрукты джокер классика"} },
    new Dictionary<string, string> { {"name","Moon Princess"}, {"provider","Play'n GO"}, {"rtp","96.50"}, {"vol","Высокая"}, {"features","Каскадные спины, 3 принцессы-бонуса, фриспины"}, {"theme","аниме принцессы магия"} },
    new Dictionary<string, string> { {"name","Rise of Olympus"}, {"provider","Play'n GO"}, {"rtp","96.50"}, {"vol","Высокая"}, {"features","Каскадные спины, 3 бога-бонуса, фриспины"}, {"theme","мифология Греция боги"} },

    // Nolimit City
    new Dictionary<string, string> { {"name","Mental"}, {"provider","Nolimit City"}, {"rtp","96.08"}, {"vol","Очень высокая"}, {"features","Расколотые символы, фриспины, множители до x9999"}, {"theme","психбольница хоррор"} },
    new Dictionary<string, string> { {"name","Tombstone RIP"}, {"provider","Nolimit City"}, {"rtp","96.08"}, {"vol","Очень высокая"}, {"features","Грязный Дикий, виселица, фриспины, x300000 макс выигрыш"}, {"theme","дикий запад"} },
    new Dictionary<string, string> { {"name","San Quentin"}, {"provider","Nolimit City"}, {"rtp","96.03"}, {"vol","Очень высокая"}, {"features","Тюремные вайлды, фриспины, множители до x150000"}, {"theme","тюрьма криминал"} },

    // Push Gaming
    new Dictionary<string, string> { {"name","Razor Shark"}, {"provider","Push Gaming"}, {"rtp","96.70"}, {"vol","Высокая"}, {"features","Стеклянные символы, множители, фриспины"}, {"theme","океан акулы дайвинг"} },
    new Dictionary<string, string> { {"name","Jammin' Jars"}, {"provider","Push Gaming"}, {"rtp","96.83"}, {"vol","Высокая"}, {"features","Каскадные спины, прыгающие вайлды, фриспины"}, {"theme","фрукты диско музыка"} },

    // Hacksaw Gaming
    new Dictionary<string, string> { {"name","Wanted Dead or a Wild"}, {"provider","Hacksaw Gaming"}, {"rtp","96.38"}, {"vol","Очень высокая"}, {"features","3 бонуса, дуэль, липкие вайлды, множители"}, {"theme","дикий запад перестрелка"} },
    new Dictionary<string, string> { {"name","Cubes 2"}, {"provider","Hacksaw Gaming"}, {"rtp","96.33"}, {"vol","Высокая"}, {"features","Кластерные выплаты, цветные бонусы, фриспины"}, {"theme","кубы минимализм"} },

    // ELK Studios
    new Dictionary<string, string> { {"name","Tahiti Gold"}, {"provider","ELK Studios"}, {"rtp","96.10"}, {"vol","Высокая"}, {"features","Каскадные символы, расширение сетки, фриспины"}, {"theme","Полинезия тропики"} },

    // Big Time Gaming
    new Dictionary<string, string> { {"name","Bonanza"}, {"provider","Big Time Gaming"}, {"rtp","96.00"}, {"vol","Высокая"}, {"features","Megaways 117649 линий, каскады, фриспины с множителем"}, {"theme","шахты золото драгоценности"} },
    new Dictionary<string, string> { {"name","Extra Chilli"}, {"provider","Big Time Gaming"}, {"rtp","96.82"}, {"vol","Высокая"}, {"features","Megaways, фриспины с gamble, множитель"}, {"theme","мексика чили специи"} },

    // Red Tiger
    new Dictionary<string, string> { {"name","Gonzo's Quest Megaways"}, {"provider","Red Tiger"}, {"rtp","96.00"}, {"vol","Высокая"}, {"features","Megaways, каскады, множители, фриспины"}, {"theme","приключения инки"} },
    new Dictionary<string, string> { {"name","Pirate's Plenty"}, {"provider","Red Tiger"}, {"rtp","95.75"}, {"vol","Средняя"}, {"features","Дикие символы, бонусные раунды, прогрессивный джекпот"}, {"theme","пираты сокровища"} },

    // Relax Gaming
    new Dictionary<string, string> { {"name","Money Train 2"}, {"provider","Relax Gaming"}, {"rtp","96.40"}, {"vol","Очень высокая"}, {"features","Бонус-бай, коллекционные символы, множители до x80000"}, {"theme","дикий запад поезд"} },
    new Dictionary<string, string> { {"name","Money Train 3"}, {"provider","Relax Gaming"}, {"rtp","96.10"}, {"vol","Очень высокая"}, {"features","Бонус-бай, новые коллекционные символы, макс выигрыш x100000"}, {"theme","дикий запад поезд"} },

    // Yggdrasil
    new Dictionary<string, string> { {"name","Vikings Go Berzerk"}, {"provider","Yggdrasil"}, {"rtp","96.10"}, {"vol","Средняя"}, {"features","Ярость викингов, фриспины, сундук сокровищ"}, {"theme","викинги скандинавия"} },
    new Dictionary<string, string> { {"name","Valley of the Gods"}, {"provider","Yggdrasil"}, {"rtp","96.20"}, {"vol","Средняя"}, {"features","Расширение линий, множители, экстра жизни"}, {"theme","Египет боги"} },

    // Thunderkick
    new Dictionary<string, string> { {"name","Esqueleto Explosivo 2"}, {"provider","Thunderkick"}, {"rtp","96.13"}, {"vol","Высокая"}, {"features","Каскадные спины, множители x32, фриспины"}, {"theme","Мексика День мёртвых"} },

    // Endorphina
    new Dictionary<string, string> { {"name","Satoshi's Secret"}, {"provider","Endorphina"}, {"rtp","96.00"}, {"vol","Средняя"}, {"features","Трейдинг-бонус, хакерский бонус, риск-игра"}, {"theme","хакеры крипто"} },

    // Evoplay
    new Dictionary<string, string> { {"name","Hot Triple Sevens"}, {"provider","Evoplay"}, {"rtp","96.01"}, {"vol","Средняя"}, {"features","Классические сёмки, множители, бонусные линии"}, {"theme","классика фрукты семёрки"} },

    // BGaming
    new Dictionary<string, string> { {"name","Elvis Frog in Vegas"}, {"provider","BGaming"}, {"rtp","96.00"}, {"vol","Средняя"}, {"features","Бесплатные спины, множители, бонусная игра"}, {"theme","Лас-Вегас лягушка"} },
    new Dictionary<string, string> { {"name","Lucky Lady Moon"}, {"provider","BGaming"}, {"rtp","97.00"}, {"vol","Средняя"}, {"features","Расширяющийся вайлд, фриспины, риск-игра"}, {"theme","луна магия ночь"} },

    // Spinomenal
    new Dictionary<string, string> { {"name","Book of Tribes"}, {"provider","Spinomenal"}, {"rtp","96.10"}, {"vol","Высокая"}, {"features","Расширяющиеся символы, фриспины, бонус-бай"}, {"theme","Египет племена"} },

    // Booongo
    new Dictionary<string, string> { {"name","Sun of Egypt 2"}, {"provider","Booongo"}, {"rtp","95.63"}, {"vol","Высокая"}, {"features","Холд энд Вин, джекпоты 3 уровня, фриспины"}, {"theme","Египет солнце"} },
    new Dictionary<string, string> { {"name","Dragon Pearls"}, {"provider","Booongo"}, {"rtp","95.65"}, {"vol","Высокая"}, {"features","Холд энд Вин, джекпоты, фриспины"}, {"theme","Азия драконы жемчуг"} },
    new Dictionary<string, string> { {"name","15 Dragon Pearls"}, {"provider","Booongo"}, {"rtp","95.53"}, {"vol","Высокая"}, {"features","Холд энд Вин, 3 джекпота, множители"}, {"theme","Азия драконы"} },

    // Amatic
    new Dictionary<string, string> { {"name","Book of Aztec"}, {"provider","Amatic"}, {"rtp","96.00"}, {"vol","Высокая"}, {"features","Расширяющийся символ, фриспины, гamble"}, {"theme","Египет ацтеки"} },

    // Belatra
    new Dictionary<string, string> { {"name","Crazy Monkey"}, {"provider","Belatra"}, {"rtp","94.00"}, {"vol","Средняя"}, {"features","Бонус с верёвками, риск-игра, супер-бонус"}, {"theme","обезьяна джунгли"} },

    // Igrosoft
    new Dictionary<string, string> { {"name","Fruit Cocktail"}, {"provider","Igrosoft"}, {"rtp","94.00"}, {"vol","Средняя"}, {"features","Бонусная игра с квадратом, риск-игра"}, {"theme","фрукты клубника классика"} },
    new Dictionary<string, string> { {"name","Garage"}, {"provider","Igrosoft"}, {"rtp","94.00"}, {"vol","Средняя"}, {"features","Бонус с ключами, риск-игра"}, {"theme","гараж машины"} },

    // Microgaming
    new Dictionary<string, string> { {"name","Immortal Romance"}, {"provider","Microgaming"}, {"rtp","96.86"}, {"vol","Высокая"}, {"features","4 типа фриспинов, сюжетные бонусы, вайлды"}, {"theme","вампиры романтика"} },
    new Dictionary<string, string> { {"name","Thunderstruck 2"}, {"provider","Microgaming"}, {"rtp","96.65"}, {"vol","Средняя"}, {"features","4 бонуса богов, прогрессивные фриспины"}, {"theme","скандинавия Тор гром"} },
    new Dictionary<string, string> { {"name","Mega Moolah"}, {"provider","Microgaming"}, {"rtp","88.12"}, {"vol","Средняя"}, {"features","Прогрессивный джекпот 4 уровня, колесо фортуны"}, {"theme","сафари Афика животные"} },

    // Blueprint Gaming
    new Dictionary<string, string> { {"name","Fishin' Frenzy"}, {"provider","Blueprint Gaming"}, {"rtp","96.12"}, {"vol","Средняя"}, {"features","Фриспины с рыбами-денежными символами, бонус-бай"}, {"theme","рыбалка море"} },

    // Quickspin
    new Dictionary<string, string> { {"name","Big Bad Wolf"}, {"provider","Quickspin"}, {"rtp","97.34"}, {"vol","Средняя"}, {"features","Каскадные спины, свинки-вайлды, фриспины с множителями"}, {"theme","сказка волк свинки"} },
    new Dictionary<string, string> { {"name","Sakura Fortune"}, {"provider","Quickspin"}, {"rtp","96.58"}, {"vol","Средняя"}, {"features","Липкие вайлды-принцессы, ре-спины, фриспины"}, {"theme","Япония сакура самурай"} },

    // Betsoft
    new Dictionary<string, string> { {"name","Gold Tiger Ascent"}, {"provider","Betsoft"}, {"rtp","96.10"}, {"vol","Средняя"}, {"features","Магические сферы, фриспины, множители"}, {"theme","Азия тигр золото"} },

    // KA Gaming
    new Dictionary<string, string> { {"name","Golden Dragon"}, {"provider","KA Gaming"}, {"rtp","96.00"}, {"vol","Средняя"}, {"features","Фриспины, множители, бонусные символы"}, {"theme","Азия дракон золото"} },

    // PG Soft
    new Dictionary<string, string> { {"name","Ways of Qilin"}, {"provider","PG Soft"}, {"rtp","96.83"}, {"vol","Средняя"}, {"features","Множественные способы выигрыша, каскады, фриспины"}, {"theme","Азия мифология цилинь"} },
    new Dictionary<string, string> { {"name","Mahjong Ways 2"}, {"provider","PG Soft"}, {"rtp","96.95"}, {"vol","Средняя"}, {"features","Маджонг-механика, множители, фриспины"}, {"theme","маджонг Азия"} },
    new Dictionary<string, string> { {"name","Lucky Neko"}, {"provider","PG Soft"}, {"rtp","95.32"}, {"vol","Средняя"}, {"features","Множественные способы, каскады, фриспины с множителями"}, {"theme","Япония манэки-нэко кошка"} },
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
        temperature = 0.8,
        top_p = 0.9
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
            Log($"    AI: ответ от {names[i]}");
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
        "Вот текст:", "Вот пост:", "---", "***"
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
// SQLite — создание таблиц
// ==========================================================================
using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={DB_PATH};Version=3;"))
{
    conn.Open();

    using (var cmd = new System.Data.SQLite.SQLiteCommand(@"
        CREATE TABLE IF NOT EXISTS posted_slots (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            slot_name TEXT,
            provider TEXT,
            post_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
            telegram_msg_id INTEGER
        );", conn))
    {
        cmd.ExecuteNonQuery();
    }
    Log("База данных готова");

    // ==========================================================================
    // Выбираем слоты которые ещё не публиковались
    // ==========================================================================
    var slotsToPost = new List<Dictionary<string, string>>();

    foreach (var slot in allSlots)
    {
        using (var checkCmd = new System.Data.SQLite.SQLiteCommand(
            "SELECT 1 FROM posted_slots WHERE slot_name=@name AND provider=@prov", conn))
        {
            checkCmd.Parameters.AddWithValue("@name", slot["name"]);
            checkCmd.Parameters.AddWithValue("@prov", slot["provider"]);
            if (checkCmd.ExecuteScalar() != null)
                continue;
        }
        slotsToPost.Add(slot);
        if (slotsToPost.Count >= SLOTS_PER_RUN) break;
    }

    if (slotsToPost.Count == 0)
    {
        Log("Все слоты уже опубликованы! Сбрасываем базу...");
        using (var resetCmd = new System.Data.SQLite.SQLiteCommand("DELETE FROM posted_slots", conn))
        {
            resetCmd.ExecuteNonQuery();
        }
        // Перебираем заново
        foreach (var slot in allSlots)
        {
            slotsToPost.Add(slot);
            if (slotsToPost.Count >= SLOTS_PER_RUN) break;
        }
    }

    Log($"Слотов к публикации: {slotsToPost.Count}");

    // ==========================================================================
    // Обрабатываем каждый слот
    // ==========================================================================
    int publishedCount = 0;

    foreach (var slot in slotsToPost)
    {
        string slotName    = slot["name"];
        string provider    = slot["provider"];
        string rtp         = slot["rtp"];
        string vol         = slot["vol"];
        string features    = slot["features"];
        string theme       = slot["theme"];

        Log($"\n--- Слот: {slotName} ({provider}) ---");

        // ---- AI: Генерация поста ----
        Log("    Генерируем пост через AI...");

        string postPrompt =
            $"Напиши привлекательный пост для Telegram-канала про азартные игры. " +
            $"Тема: обзор слота \"{slotName}\" от {provider}.\n\n" +
            $"Характеристики слота:\n" +
            $"- RTP: {rtp}%\n" +
            $"- Волатильность: {vol}\n" +
            $"- Фичи: {features}\n" +
            $"- Тематика: {theme}\n\n" +
            $"Требования к посту:\n" +
            $"1. Начни с цепляющего заголовка с эмодзи (например: 🔥 или 🎰 или 💎)\n" +
            $"2. Кратко опиши слот — чем интересен, какие фичи выделяют\n" +
            $"3. Упомяни RTP и волатильность простым языком\n" +
            $"4. Опиши бонусные фичи и фриспины — что игрок получит\n" +
            $"5. Заканчивай призывом к действию (крути, пробуй, играй)\n" +
            $"6. Текст должен быть живой, разговорный, без воды\n" +
            $"7. НЕ пиши заголовок типа 'Обзор слота' — сразу цепляй\n" +
            $"8. Длина: 400-700 символов\n" +
            $"9. Верни ТОЛЬКО текст поста, без пояснений\n" +
            $"10. НЕ добавляй теги и хештеги — я добавлю сам";

        string postText = CallAI(postPrompt);

        if (string.IsNullOrWhiteSpace(postText))
        {
            Log("    AI не сгенерировал пост, пропускаем");
            continue;
        }

        postText = CleanResponse(postText);

        // ---- Добавляем ссылку и теги ----
        string[] themeWords = theme.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
        var tagList = new List<string> { "слоты", "казино" };
        if (themeWords.Length > 0) tagList.Add(themeWords[0]);
        if (themeWords.Length > 1) tagList.Add(themeWords[1]);
        tagList = tagList.Take(4).ToList();
        string tagsText = string.Join(" ", tagList.Select(t => $"#{t}"));

        string caption = $"{postText}\n\n🎮 Играть в {CASINO_NAME} → {PARTNER_LINK}\n\n{tagsText}";

        if (caption.Length > CAPTION_MAX_LEN)
        {
            int maxText = CAPTION_MAX_LEN - ($"\n\n🎮 Играть в {CASINO_NAME} → {PARTNER_LINK}\n\n{tagsText}").Length - 10;
            if (maxText < 100) maxText = 100;
            if (postText.Length > maxText)
                postText = postText.Substring(0, maxText) + "...";
            caption = $"{postText}\n\n🎮 Играть в {CASINO_NAME} → {PARTNER_LINK}\n\n{tagsText}";
        }

        Log($"    Пост: {caption.Length} символов");

        // ---- Ищем картинку слота ----
        string imageUrl = "";
        try
        {
            string searchQuery = Uri.EscapeDataString($"{slotName} {provider} slot game");
            string searchUrl = $"https://html.duckduckgo.com/html/?q={searchQuery}";

            try { var _ = httpClient.GetStringAsync("https://duckduckgo.com/").Result; }
            catch { }
            System.Threading.Thread.Sleep(1000);

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
                        imageUrl = src;
                        break;
                    }
                }
            }
        }
        catch { }

        // Фоллбэк — заглушка с названием слота
        if (string.IsNullOrEmpty(imageUrl))
            imageUrl = $"https://placehold.co/600x400/1a1a2e/e94560?text={Uri.EscapeDataString(slotName)}";

        Log($"    Картинка: {(imageUrl.Length > 60 ? imageUrl.Substring(0, 60) + "..." : imageUrl)}");

        // ---- Отправка в Telegram ----
        Log("    Отправляем в Telegram...");

        bool sent = false;
        int msgId = 0;

        // Попытка 1: sendPhoto
        try
        {
            string tgUrl = $"https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendPhoto";
            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                chat_id    = TELEGRAM_CHANNEL_ID,
                photo      = imageUrl,
                caption    = caption,
                parse_mode = "Markdown"
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
                Log($"    ✅ sendPhoto: msg_id={msgId}");
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

        // Попытка 2: sendMessage
        if (!sent)
        {
            try
            {
                string tgUrl = $"https://api.telegram.org/bot{TELEGRAM_BOT_TOKEN}/sendMessage";
                var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    chat_id    = TELEGRAM_CHANNEL_ID,
                    text       = caption,
                    parse_mode = "Markdown"
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
                    Log($"    ✅ sendMessage: msg_id={msgId}");
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

        // ---- Сохраняем в БД ----
        if (sent)
        {
            using (var insertCmd = new System.Data.SQLite.SQLiteCommand(
                "INSERT INTO posted_slots (slot_name, provider, telegram_msg_id) VALUES (@name, @prov, @mid)", conn))
            {
                insertCmd.Parameters.AddWithValue("@name", slotName);
                insertCmd.Parameters.AddWithValue("@prov", provider);
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

        // Пауза между слотами
        if (publishedCount < slotsToPost.Count)
        {
            Log("    Пауза 5 сек...");
            System.Threading.Thread.Sleep(5000);
        }
    }

    Log($"\n========== ИТОГО: опубликовано {publishedCount} из {slotsToPost.Count} ==========");
}

httpClient.Dispose();
httpHandler.Dispose();

return "OK";
