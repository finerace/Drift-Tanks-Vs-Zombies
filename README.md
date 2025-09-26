<p align="right">
  <img src="https://flagpedia.net/data/flags/w20/gb.png" width="20" alt="English">
  <a href="#tldr-1"> English Version</a>
  <a href="https://github.com/FineRace">
  <img src="https://i.postimg.cc/nzjMnxmF/mini-icon.png" width="30" alt="icon" align="left">
</p>

<p align="center">
  <img src="https://i.postimg.cc/nLGBmg1m/logoru.png" width="450" alt="Tanks on Yandex Logo">
</p>

<p align="center">
  <a href="https://www.linkedin.com/in/finerace/" target="_blank">
    <img src="https://img.shields.io/badge/LinkedIn-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn"/>
  </a>
  <a href="https://t.me/finerace" target="_blank">
    <img src="https://img.shields.io/badge/Telegram-26A5E4?style=for-the-badge&logo=telegram&logoColor=white" alt="Telegram"/>
  </a>
</p>

---

<p align="center">
  <img src="https://i.postimg.cc/26XqFDLT/screen-1.png" width="400" alt="Gameplay Screenshot 1">
  <img src="https://i.postimg.cc/G2MD7DVv/screen-2.png" width="400" alt="Gameplay Screenshot 2">
</p>

---

### TL;DR
- 🤖 **Сложный алгоритм процедурной генерации:** Разработал с нуля многоэтапный алгоритм для создания уникальных городских уровней (`LevelGenerator.cs`), что ускорило процесс создания контента и уменьшило размер префабов уровней.
- 🔌 **Интеграция с внешним API:** Реализовал полную поддержку **Yandex.Games SDK**, включая авторизацию, облачные сохранения, лидерборды, внутриигровые покупки и вознаграждаемую рекламу. Есть опыт "сырого" API-взаимодействия.
- 🚗 **Продвинутая физика танка:** Создал кастомный контроллер на `Rigidbody` с реалистичной симуляцией работы подвески (`TankCorpusBouncing.cs`), инерцией и механикой дрифта.
- 🔊 **Оптимизированный аудио-менеджер:** Написал пул аудио-источников (`AudioPoolService.cs`) с **системой приоритетов**, которая "ворует" менее важные каналы для проигрывания критически важных звуков, предотвращая потерю фидбэка в напряженных сценах.
- 📊 **Data-driven магазин:** Спроектировал систему магазина и апгрейдов на `ScriptableObject`-конфигах (`ShopData.cs`, `TanksShopService.cs`), что позволяет легко балансировать и добавлять новый контент без изменения кода.
- 💥 **Кастомная физика взрывов:** Реализовал логику взрывов (`Explosions` в `AuxularityFunc.cs`), которая учитывает прямую видимость до цели (проверка через Raycast) и поддерживает направленное действие.

---

<p align="center">
  <img src="https://i.postimg.cc/yxgDLbFK/screen-3.png" width="400" alt="Gameplay Screenshot 3">
  <img src="https://i.postimg.cc/5yYXsR8g/screen-4.png" width="400" alt="Gameplay Screenshot 4">
</p>

---

### 💡 О проекте

Этот проект был создан как полигон для решения двух ключевых задач: реализации сложного алгоритма процедурной генерации и полной интеграции игры с внешней веб-платформой. В отличие от моих более поздних работ, здесь архитектура построена на классических для Unity подходах (синглтоны, прямые ссылки), что потребовало "ручного" управления зависимостями и состоянием.

---

### 🧠 Кейс-стади: Процедурная генерация городских уровней

Одной из главных целей проекта была возможность быстро создавать новые, уникальные уровни без необходимости вручную собирать их из сотен префабов. Это привело к созданию кастомного генератора, который строит целый город с нуля.

> [!NOTE]
> <details>
>   <summary><strong>Показать разбор алгоритма...</strong></summary>
>
>   # 🤖 Проектирование алгоритма генерации
>
>   ## 1. 🎯 Проблема и Цели
>
>   *   **Скорость:** Ручное создание разнообразных уровней — это долго.
>   *   **Размер проекта:** Хранение десятков уникальных префабов уровней значительно увеличивает размер билда.
>   *   **Реиграбельность:** Игрокам быстро надоедают одни и те же карты.
>
>   **Цель:** Создать алгоритм, который по одному `seed` генерирует полноценный играбельный уровень с дорожной сетью, городскими кварталами и врагами.
>
>   ## 2. ⚙️ Архитектура и Реализация (`LevelGenerator.cs`)
>
>   Процесс генерации разбит на несколько логических этапов:
>
>   *   **Этап 1: Генерация дорожной сети.**
>       *   **Подход:** Используется алгоритм "блуждающих точек" (`generationPoints`). На старте в центре карты создается несколько точек-генераторов.
>       *   **Логика:** На каждом шаге итерации точки движутся вперед, "рисуя" за собой путь на 2D-массиве `levelMap`. С определенной вероятностью (`pointRotateChance`, `newPointGenerateChance`) они могут повернуться или создать новую точку-потомка, которая начнет строить ответвление дороги. Точки "умирают", если выходят за пределы карты или врезаются в уже существующую дорогу.
>
>   *   **Этап 2: Выбор правильных тайлов.**
>       *   **Подход:** После того как дорожная сеть "нарисована", алгоритм проходит по каждой ячейке `levelMap`.
>       *   **Логика:** Для каждой ячейки с дорогой вызывается функция `GetCellEnvironmentCof`, которая анализирует её соседей (есть ли дорога сверху, снизу, слева, справа). На основе этой информации (например, "дорога есть только на севере и юге") система выбирает из `RoadsPrefs` соответствующий префаб: прямой участок, поворот, Т-образный перекресток или четырехсторонний перекресток.
>
>   *   **Этап 3: Заполнение окружения.**
>       *   **Подход:** Мир не должен состоять только из дорог. Все пустые ячейки нужно заполнить соответствующим окружением.
>       *   **Логика:** Для каждой пустой ячейки вычисляется её кратчайшее расстояние до ближайшей дороги. В зависимости от этого расстояния ячейка помечается как "городской квартал" (`cityDepth`, рядом с дорогой) или "граница карты" (`bordersDepth`, далеко от дороги).
>
>   *   **Этап 4: Финальный спавн.**
>       *   **Подход:** На последнем этапе логическая карта `levelMap` превращается в реальные `GameObjects` на сцене.
>       *   **Логика:** Система инстанциирует префабы дорог, городских зданий и границ в нужных координатах. После этого на дорожных участках с определенным шансом (`zombieSpawnChance`) размещаются враги.
>
>   ## 3. ✅ Результаты и Выводы
>
>   *   **Результат:** Был создан мощный инструмент, который позволяет генерировать бесконечное количество вариаций уровней, изменяя всего один параметр — `seed`.
>   *   **Полученный опыт:** Этот процесс научил меня решать сложные алгоритмические задачи, работать с многомерными массивами для представления игрового мира и разбивать одну большую задачу (генерация уровня) на управляемые, последовательные этапы.
>
> </details>

---

### 🔌 О интеграция с API платформы Yandex.Games

Игра была опубликована на платформе Яндекс.Игры, что потребовало глубокой интеграции с её SDK. Это была практическая задача по связыванию внутренних игровых систем с внешним API.

> [!NOTE]
> <details>
>   <summary><strong>Показать детали интеграции...</strong></summary>
>
>   # 🔗 Работа с Yandex.Games SDK
>
>   ## 1. 🎯 Задача
>
>   Превратить оффлайн-игру в полноценный онлайн-продукт, используя все возможности платформы для улучшения пользовательского опыта и монетизации.
>
>   ## 2. ⚙️ Реализованные интеграции
>
>   *   **Авторизация и данные игрока (`AuthPlayerPanel.cs`):**
>       *   Реализована проверка статуса авторизации игрока.
>       *   После успешной авторизации, с помощью API получается и отображается в UI имя и аватар пользователя (`YandexGame.playerName`, `YandexGame.playerPhoto`).
>
>   *   **Облачные сохранения (`GameDataSaver.cs`):**
>       *   Весь прогресс игрока (рекорды на уровнях, купленные танки, улучшения, валюта) сохраняется не локально, а на серверах Яндекса через `YandexGame.SaveProgress()`.
>       *   При старте игры данные загружаются через `YandexGame.GetDataEvent`, что обеспечивает синхронизацию прогресса между устройствами.
>
>   *   **Лидерборды (`GameDataSaver.cs`):**
>       *   После прохождения уровня общий счёт игрока отправляется в глобальную таблицу рекордов через `YandexGame.NewLeaderboardScores()`.
>
>   *   **Внутриигровые покупки (`PlayerMoneyXpService.cs`):**
>       *   Реализована логика для инициации покупки (`YandexGame.BuyPayments`).
>       *   Создан обработчик колбэка `YandexGame.PurchaseSuccessEvent`, который начисляет игроку купленную валюту после успешной транзакции.
>
>   *   **Вознаграждаемая реклама (Rewarded Ads) (`PlayerMoneyXpService.cs`):**
>       *   Внедрено несколько сценариев использования рекламы: возрождение игрока после смерти, удвоение награды за уровень, получение дополнительного бонуса.
>       *   Реализован обработчик колбэка `YandexGame.RewardVideoEvent`, который по ID рекламы определяет, какую именно награду выдать игроку.
>
>    Помимо работы с готовым плагином (как в этом проекте), у меня также есть опыт "сырой" интеграции с API через прямые HTTP-запросы, в частности для авторизации и отправки данных на сервер в другом проекте для той же платформы.
>
> </details>

---

### 🛠️ Другие реализованные системы

> [!NOTE]
> <details>
>   <summary><strong>Показать список...</strong></summary>
>
>   *   **Физика и управление танком (`PlayerTank.cs`)**
>       *   Контроллер полностью основан на `Rigidbody` и применении сил/крутящих моментов, что дает ощущение инерции и веса.
>       *   Реализована механика дрифта через смену физических материалов у коллайдеров колес (`normalMaterial` / `driftMaterial`).
>       *   Создана система "баунсинга" корпуса (`TankCorpusBouncing.cs`), которая имитирует работу подвески, визуально реагируя на ускорение и скорость танка.
>
>   *   **Система локализации (`LanguageData.cs`, `CurrentLanguageData.cs`)**
>       *   Написан простой парсер, который читает текстовый файл `.txt`, разделенный символом `;`, и загружает все строки в массив. Это позволяет хранить все тексты для одного языка в одном ассете и легко переключаться между ними.
>
> </details>

<h3 align="center">Спасибо за внимание!</h3>

<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>

<p align="right">
  <img src="https://flagpedia.net/data/flags/w20/ru.png" width="20" alt="Русский">
  <a href="#tldr"> Русская версия</a>
  <a href="https://github.com/FineRace">
  <img src="https://i.postimg.cc/nzjMnxmF/mini-icon.png" width="30" alt="icon" align="left">
</p>

<p align="center">
  <img src="https://i.postimg.cc/DyRr3qZb/logoen.png" width="450" alt="Tanks on Yandex Logo">
</p>

<p align="center">
  <a href="https://www.linkedin.com/in/finerace/" target="_blank">
    <img src="https://img.shields.io/badge/LinkedIn-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white" alt="LinkedIn"/>
  </a>
  <a href="https://t.me/finerace" target="_blank">
    <img src="https://img.shields.io/badge/Telegram-26A5E4?style=for-the-badge&logo=telegram&logoColor=white" alt="Telegram"/>
  </a>
</p>

---

<p align="center">
  <img src="https://i.postimg.cc/26XqFDLT/screen-1.png" width="400" alt="Gameplay Screenshot 1">
  <img src="https://i.postimg.cc/G2MD7DVv/screen-2.png" width="400" alt="Gameplay Screenshot 2">
</p>

---

### TL;DR
- 🤖 **Complex Procedural Generation Algorithm:** Developed a multi-stage algorithm from scratch to create unique city levels (`LevelGenerator.cs`), which accelerated the content creation pipeline and reduced the size of level prefabs.
- 🔌 **External API Integration:** Implemented full support for the **Yandex.Games SDK**, including authentication, cloud saves, leaderboards, in-app purchases, and rewarded ads. Also have experience with raw API communication.
- 🚗 **Advanced Tank Physics:** Created a custom `Rigidbody`-based controller with a realistic suspension simulation (`TankCorpusBouncing.cs`), inertia, and a drifting mechanic.
- 🔊 **Optimized Audio Manager:** Wrote an audio source pool (`AudioPoolService.cs`) with a **priority system** that "steals" less important channels to play critical sounds, preventing loss of feedback in intense moments.
- 📊 **Data-Driven Shop:** Designed a shop and upgrade system based on `ScriptableObject` configs (`ShopData.cs`, `TanksShopService.cs`), allowing for easy balancing and content additions without changing code.
- 💥 **Custom Explosion Physics:** Implemented explosion logic (`Explosions` in `AuxularityFunc.cs`) that accounts for line-of-sight to the target (via Raycast checks) and supports directional force.

---

<p align="center">
  <img src="https://i.postimg.cc/yxgDLbFK/screen-3.png" width="400" alt="Gameplay Screenshot 3">
  <img src="https://i.postimg.cc/5yYXsR8g/screen-4.png" width="400" alt="Gameplay Screenshot 4">
</p>

---

### 💡 About the Project

This project was developed as a testing ground for solving two key challenges: implementing a complex procedural generation algorithm and fully integrating a game with an external web platform. Unlike my later work, this project's architecture is built on classic Unity patterns (singletons, direct references), which required manual dependency and state management.

---

### 🧠 Case Study: Procedural Generation of City Levels

One of the main goals was to enable the rapid creation of new, unique levels without having to manually assemble them from hundreds of prefabs. This led to the development of a custom generator that builds an entire city from the ground up.

> [!NOTE]
> <details>
>   <summary><strong>Show the algorithm breakdown...</strong></summary>
>
>   # 🤖 Designing the Generation Algorithm
>
>   ## 1. 🎯 Problem and Goals
>
>   *   **Speed:** Manually creating a variety of levels is time-consuming.
>   *   **Project Size:** Storing dozens of unique level prefabs significantly increases the build size.
>   *   **Replayability:** Players quickly get bored of the same maps.
>
>   **Goal:** Create an algorithm that generates a complete, playable level with a road network, city blocks, and enemies from a single `seed`.
>
>   ## 2. ⚙️ Architecture and Implementation (`LevelGenerator.cs`)
>
>   The generation process is divided into several logical stages:
>
>   *   **Stage 1: Road Network Generation.**
>       *   **Approach:** A "wandering points" algorithm (`generationPoints`) is used. At the start, several generator points are created in the center of the map.
>       *   **Logic:** In each iteration, these points move forward, "drawing" a path on a 2D array (`levelMap`). With a certain probability (`pointRotateChance`, `newPointGenerateChance`), they can turn or spawn a new child point that will start building a road branch. Points "die" if they go off the map or collide with an existing road.
>
>   *   **Stage 2: Selecting the Correct Tiles.**
>       *   **Approach:** After the road network is "drawn," the algorithm iterates through each cell of the `levelMap`.
>       *   **Logic:** For each road cell, the `GetCellEnvironmentCof` function is called to analyze its neighbors (is there a road to the north, south, east, west?). Based on this information (e.g., "road exists only to the north and south"), the system selects the appropriate prefab from `RoadsPrefs`: a straight section, a turn, a T-junction, or a four-way intersection.
>
>   *   **Stage 3: Filling the Environment.**
>       *   **Approach:** The world shouldn't consist only of roads. All empty cells must be filled with appropriate surroundings.
>       *   **Logic:** For each empty cell, its shortest distance to the nearest road is calculated. Depending on this distance, the cell is marked as a "city block" (`cityDepth`, near the road) or a "map border" (`bordersDepth`, far from the road).
>
>   *   **Stage 4: Final Spawning.**
>       *   **Approach:** In the final stage, the logical `levelMap` is converted into actual `GameObjects` in the scene.
>       *   **Logic:** The system instantiates prefabs for roads, city buildings, and borders at the correct coordinates. After this, enemies are placed on the road sections with a certain chance (`zombieSpawnChance`).
>
>   ## 3. ✅ Results and Conclusions
>
>   *   **Result:** A powerful tool was created that allows generating an infinite number of level variations by changing just a single parameter—the `seed`.
>   *   **Experience Gained:** This process taught me how to solve complex algorithmic problems, work with multidimensional arrays to represent the game world, and break down a large task (level generation) into manageable, sequential stages.
>
> </details>

---

### 🔌 About Integrating with the Yandex.Games Platform API

The game was published on the Yandex.Games platform, which required deep integration with its SDK. This was a practical task of connecting internal game systems with an external API.

> [!NOTE]
> <details>
>   <summary><strong>Show integration details...</strong></summary>
>
>   # 🔗 Working with the Yandex.Games SDK
>
>   ## 1. 🎯 The Task
>
>   To transform an offline game into a full-fledged online product, leveraging all platform features to enhance user experience and monetization.
>
>   ## 2. ⚙️ Implemented Integrations
>
>   *   **Authentication and Player Data (`AuthPlayerPanel.cs`):**
>       *   Implemented a check for the player's authentication status.
>       *   After successful authentication, the API is used to retrieve and display the user's name and avatar in the UI (`YandexGame.playerName`, `YandexGame.playerPhoto`).
>
>   *   **Cloud Saves (`GameDataSaver.cs`):**
>       *   All player progress (level high scores, purchased tanks, upgrades, currency) is saved to Yandex servers via `YandexGame.SaveProgress()` instead of locally.
>       *   On game start, data is loaded via the `YandexGame.GetDataEvent`, ensuring progress is synced across devices.
>
>   *   **Leaderboards (`GameDataSaver.cs`):**
>       *   After completing a level, the player's total score is submitted to the global leaderboard via `YandexGame.NewLeaderboardScores()`.
>
>   *   **In-App Purchases (`PlayerMoneyXpService.cs`):**
>       *   Implemented logic to initiate a purchase (`YandexGame.BuyPayments`).
>       *   Created a callback handler for `YandexGame.PurchaseSuccessEvent` to grant the player the purchased currency after a successful transaction.
>
>   *   **Rewarded Ads (`PlayerMoneyXpService.cs`):**
>       *   Integrated several ad placement scenarios: reviving the player after death, doubling the level reward, and receiving an extra bonus.
>       *   Implemented a callback handler for `YandexGame.RewardVideoEvent` that uses the ad ID to determine which specific reward to grant the player.
>
>   In addition to working with a pre-built plugin (as in this project), I also have experience with "raw" API integration via direct HTTP requests, specifically for authentication and data submission in another project for the same platform.
>
> </details>

---

### 🛠️ Other Implemented Systems

> [!NOTE]
> <details>
>   <summary><strong>Show the list...</strong></summary>
>
>   *   **Tank Physics and Controls (`PlayerTank.cs`)**
>       *   The controller is entirely `Rigidbody`-based, using forces and torques to create a sense of inertia and weight.
>       *   A drifting mechanic was implemented by swapping the `PhysicMaterial` on the wheel colliders (`normalMaterial` / `driftMaterial`).
>       *   A "corpus bouncing" system (`TankCorpusBouncing.cs`) was created to simulate suspension, visually reacting to the tank's acceleration and speed.
>
>   *   **Localization System (`LanguageData.cs`, `CurrentLanguageData.cs`)**
>       *   Wrote a simple parser that reads a `.txt` file delimited by semicolons (`;`) and loads all strings into an array. This allows all text for a single language to be stored in one asset, making it easy to switch between them.
>
> </details>

<h3 align="center">Thank you for your attention!</h3>
