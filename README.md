# SCOM-CFU (WIP)
A tool to enrich SCOM alerts by adding data to the custom fields. You can use this to assign support team info to alerts or to add other static or dynamic data.
Static data is stored in a sqlite database. Dynamic data can be added by embedding powershell scripts, the script output will then be inserted into the customfield.
You can configure the custom field data per alert, target or management pack. This makes it less time consuming when configuring new management packs where all alerts have to go to the same support team or need the same customfield data.
