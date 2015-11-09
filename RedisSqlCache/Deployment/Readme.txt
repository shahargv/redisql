---------------------------------
RediSql - T-SQL client for Redis
---------------------------------

README

* RediSql provided as-is, with no guaranees at all. It's your responsibility to take all the necessary precautions before installing it.
* RediSql installation is fairly simple:
	1. Copy all text from install.sql to text editor (SQL Server Management Studio recommended)
	2. Change the first line - replace "check1" with your DB name
	3. Make sure you run the script on the DB you want it, either by using sqlcmd and using the "-d" flag, or by choosing the DB name from
		the combo box in SSMS
	4. Execute the statements.

* How the installation script is going to affect your DB?
	- New schema, called "redisql" will created in your DB with the same permissions as the dbo schema
	- 2 SQL CLR assemblies will be added to your database
	- SQL CLR will be enabled in the server
	- Multiple functions and stored procedures will be added, all under "redisql" schema
	- (for a full list please refer to the official site, http://redisql.ishahar.net )

* Where can you get more information?
	- Official website: http://redisql.ishahar.net
	- GitHub (for the source code): http://github.com/shahargv/redisql

Hope you'll enjoy RediSql :)

	

