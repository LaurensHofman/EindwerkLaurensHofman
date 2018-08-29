# EindwerkLaurensHofman
Eindwerk Laurens Hofman E-Commerce

Omdat momenteel mijn database nog niet online staat, zijn er wel nog een aantal stappen nodig om het bij u lokaal te kunnen testen.

1. Een backup file van m'n SQL database (CProefRudy.bak) staat hier samen met m'n project op GitHub
  U kan bijgevolg in de MSSQL Management Studio de databank restoren uit deze file
  (Rechterklik op Databases, Restore Database, Source = Device, Add Backup Media, selecteer de file, en dan OK tot het gerestored is)
  
2. In mijn code zal waarschijnlijk wel nog de connection string upgedated moeten worden, afhankelijk van het pad bij u naar m'n databank
  (Dit moet op 2 plaatsen gebeuren: de App.config in het RudycommerceWPF project; en in de Web.config van het RudycommerceWeb project)

3. Indien de WPF en de Website niet tegelijk opstarten, zal u in de Solution Explorer bij de Solution meerde Startup projects moeten kiezen,
  met name de RudycommerceWEB en de RudycommerceWPF
  
Normaal gezien zouden nu mijn website en WPF moeten werken.

Met dank,
Laurens Hofman
VDO Analist Programmeur
