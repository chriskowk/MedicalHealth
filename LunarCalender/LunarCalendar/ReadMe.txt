DROP TABLE Diary;

CREATE TABLE [Diary](
  [ID] INTEGER NOT NULL, 
  [Title] NVARCHAR2(200) NOT NULL DEFAULT '' COLLATE NOCASE, 
  [Keywords] NVARCHAR2(200) NOT NULL DEFAULT '' COLLATE NOCASE, 
  [Content] NVARCHAR2(8000) NOT NULL DEFAULT '' COLLATE NOCASE, 
  [RecordDate] DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
  [IsRemindRequired] BOOL NULL NULL DEFAULT 0,
  [CronExpress] VARCHAR(128) NOT NULL DEFAULT '' COLLATE NOCASE,
  [JobGroup] VARCHAR(128) NOT NULL DEFAULT 'TaskGroup' COLLATE NOCASE,
  [JobName] VARCHAR(128) NOT NULL DEFAULT '' COLLATE NOCASE,
  [JobTypeFullName] VARCHAR(128) NOT NULL DEFAULT '' COLLATE NOCASE,
  [RunningStart] DATETIME,
  [RunningEnd] DATETIME,
  [RowVersion] DATETIME NOT NULL DEFAULT (datetime('now','localtime')),
  [IsDeleted] 	BOOL NULL NULL DEFAULT 0,
  PRIMARY KEY([ID] COLLATE [BINARY] ASC AUTOINCREMENT) ON CONFLICT ROLLBACK
);

本程序运行环境-数据库DBMS: SQLite; 数据库名:Calendar.db
常用SQLite查询语句： 
select STRFTIME('%Y-%m-%d %H:%M', RecordDate, 'localtime'), * from diary 
update diary set IsRemindRequired=IsRemindRequired, RowVersion=datetime('now','localtime'), RecordDate='2020-09-16 00:00:00' where ID = 7;
UPDATE Diary SET IsRemindRequired = 0 WHERE IsRemindRequired = 1 AND (RecordDate < '2020-05-01' OR (RecordDate = '2020-05-01' AND RecordDate < '1899-12-30 11:54'))
select * from diary WHERE RemindFlag = 1 AND (RecordDate < '2020-05-01' OR (RecordDate = '2020-05-01' AND RecordDate < '1899-12-30 11:54'))

UPDATE Diary SET RemindFlag = 0 WHERE RemindFlag = 1 AND DATE(RecordDate) = '2020-06-01' AND STRFTIME('%H:%M', RecordDate, 'localtime') = '15:01'

select * from diary WHERE RemindFlag = 1 AND DATE(RecordDate) = '2020-05-30' AND TIME(RecordDate) = '15:01:00'

select STRFTIME('%h:%m', RecordDate, 'localtime'), * from diary where STRFTIME('%H:%M', RecordDate, 'localtime') = '15:01';

update Diary set RecordDate = time('15:01') where id = 1;

SELECT strftime( '%Y-%m-%d %H:%M', 'now', 'localtime');

insert into diary(Title, Keywords, Content, RecordDate) values('lafdk','fdke','AAAAAA','2020-5-30 18:00');


ALTER TABLE [Diary] ADD [JobGroup] VARCHAR(128) NOT NULL DEFAULT '' COLLATE NOCASE;
ALTER TABLE [Diary] ADD [JobName] VARCHAR(128) NOT NULL DEFAULT '' COLLATE NOCASE;
