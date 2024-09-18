# EventProcessorITentika
09/24 ITentika Тестовое задание .NET

## Начало работы
1. Склонируйте репозиторий
    ```
    git clone https://github.com/neuron1st/EventProcessorITentika
    ```

2. Настройте строку подключения к базе данных в ```Processor\appsettings.json```
    ```
    "ProcessorDb": "Server=localhost;Port=5432;Database=EventProcessor;User Id=postgres;Password= ;"
    ```

3. Обновите базу данных в Package Manager Console следующей командой, выбрав в качестве проекта по умолчанию ```Processor```
    ```
    Update-Database
    ```

4. Запустите сервис Processor

5. Запустите сервис Generator