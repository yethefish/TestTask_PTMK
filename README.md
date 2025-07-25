# TestTask_PTMK

> [!TIP]
> Для запуска: dotnet run
> 
> Перед запуском нужно изменить строку DefaultConnection в appsettings.json

## Общие требования:
- Написать приложение, которое будет запускаться из консоли с параметрами. Первый параметр, передаваемый при запуке, выбирает режим работы приложения соответствующий пункту задания. По ходу задания будут примеры запуска приложения.
- Для ФИО использовать английский язык. Решать проблему с отображением русского языка в консоли, если возникает, не нужно.
- Приложение должно подключаться к базе данных.
- В качестве СУБД можно использовать любую SQL СУБД или MongoDB.
- В разработке приложения обязательно применять объектно-ориентированный подход.
- В качестве среды разработки можете использовать любой известный вам объектно-ориентированный язык программирования.

## Режимы работы приложения:
### 1. Создание таблицы с полями справочника сотрудников, представляющими "Фамилию Имя Отчество", "дату рождения", "пол".

Пример запуска приложения:
myApp 1
К примеру для java:
java myApp.class 1 или java myApp.jar 1

### 2. Создание записи справочника сотрудников.
Для работы с данными создать класс и создавать объекты. При вводе создавать новый объект класса, с введенными пользователем данными.
При генерации строчек в базу создавать объект и его отправлять в базу/формировать строчку для отправки нескольких строк в БД.
У объекта должны быть методы, которые:
- отправляют объект в БД,
- рассчитывают возраст (полных лет).

Пример запуска во 2 режиме:
myApp 2 "Ivanov Petr Sergeevich" 2009-07-12 Male

### 3. Вывод всех строк справочника сотрудников, с уникальным значением ФИО+дата, отсортированным по ФИО. Вывести ФИО, Дату рождения, пол, кол-во полных лет.
Пример запуска приложения:
myApp 3

### 4. Заполнение автоматически 1000000 строк справочника сотрудников. Распределение пола в них должно быть относительно равномерным, начальной буквы ФИО также. Добавить заполнение автоматически 100 строк в которых пол мужской и Фамилия начинается с "F".
У класса необходимо создать метод, который пакетно отправляет данные в БД, принимая массив объектов.
Пример запуска приложения:
myApp 4

### 5. Результат выборки из таблицы по критерию: пол мужской, Фамилия начинается с "F". Сделать замер времени выполнения.
Пример запуска приложения:
myApp 5
Вывод приложения должен содержать время. Заполнить это время в отчете по выполнению тестового задания.

### 6. Произвести оптимизацию базы данных или запросов к базе для ускорения выполнения пункта 5. Убедиться, что время исполнения уменьшилось. Объяснить смысл произведенных действий. Предоставить результаты замера времени выполнения до и после оптимизации.
