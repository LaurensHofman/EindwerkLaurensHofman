# EindwerkLaurensHofman
Eindwerk Laurens Hofman E-Commerce

Currently my database is not online yet, so there are a few steps to follow to be able to test my program locally.

1. A backup file of my SQL database (CProefRudy.bak) is included with the project here on GitHub.
  In MSSQL Management Studio, you can restore the database from this file.
  (One way to do it is: Right click on 'Databases', 'Restore Database...', 'Source = Device', 'Add Backup Media', select the file, and click OK
  until the file is restored
  
2. In my project's code the connection string will probably have to be updated, dependent on the path to the database.
  (This needs to happen in 2 places: the App.config in the RudycommerceWPF project; and in the Web.config in the RudycommerceWeb project)

3. In case the WPF and the Website don't start up together, you can right click the Solution in the Solution Explorer and select the Startup Projects, which need to be RudycommerceWeb and RudycommerceWPF)

Usually the website and the Wpf should work by now.

On the website, the payments happen in a sandbox (so no real payments are happening), and you need to fill in one of the default card numbers (defined by Stripe), for example 4242 4242 4242 4242 for VISA, together with a random valid expiration date and a random CVC number

Thank you very much,
Laurens Hofman
VDO Analist Programmeur
