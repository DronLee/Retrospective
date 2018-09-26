# Retrospective
Проект создавался как пример, на котором можно попробовать платформу ASP.NET Core.
Так что большого прикладного значения не имеет, скорее демонстрирует основные технологии данной платформы.
- ASP.NET MVC (по классической схеме реализовано взаимодействие моделей, представлений и контроллеров).
- Промежуточное ПО (всё является промежуточным ПО).
- Entity Framework Core (работа с БД полностью реализована через данную технологию).
- Razor (прям в html разметке используются циклы, условия, обращение к свойствам модели).
- TagHelpers (позволяют легко настроить соответствия между свойствами модели и элементами представления).
И лёгкость в реализации таких важных вещей, как:
- Интернационализация (выбор языков, которые легко при необходимости дополняются).
- Журналирование (все основные действия контроллеров записываются в журнал).

Проект представляет собой сайт для организации ретроспективы.
Суть ретроспективы - анализировать пройденный этап и ставить задачи на будущее.
В данном случае это делается по трём показателям: "Начать", "Прекратить", "Продолжать".
Например, работая по методологии SCRUM, можно в конце каждого спринта просить сотрудников заполнять таблицу из этих полей, и использовать полученные мнения при подготовки следующих спринтов.
Или лично перед собой ставить какие то цели и нблюдать за их реализацией.
Для этого сначала предлагается создать тему или войти в имеющуюся (например, "SCRUM"). Естественно, с паролем.
В теме в первую очередь отображаются записи за текущий день, но можно выбрать и любой (на выбор даётся не больше 20 последних) из дней за которые были раннее сделаны записи.
Добавлять новые записи можно только для текущего дня.
Всё.

Для запуска требуется лишь платформа .NET Core 2 или более поздняя версия (является кроссплатформенной и легко ставится и на Windows и на Unix) и SQL-сервер, для хранения БД. Запускается в несколько команд, как описано в DockerFile:
dotnet restore (восстановление внешних зависимостей)
dotnet build (сборка проекта)
dotnet ef database update (выполнение всех миграций БД и её создание в случае отсутствия)
dotnet run (запуск проекта)
