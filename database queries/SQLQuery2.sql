SELECT name
FROM sys.databases;

create database rmcdonate;

use rmcdonate;

CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    fullname VARCHAR(255) NOT NULL,
    dob DATE NOT NULL,
    address VARCHAR(255) NOT NULL,
    profession VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    mobile_no VARCHAR(20) UNIQUE NOT NULL,
    profilephoto VARCHAR(255),
    createdat DATETIME DEFAULT GETDATE(),
    updatedat DATETIME DEFAULT GETDATE(),
    lastlogin DATETIME DEFAULT GETDATE(),
    status INT DEFAULT 1 CHECK (status BETWEEN 0 AND 4)
);

ALTER TABLE users
ADD VerificationToken VARCHAR(50);

UPDATE users SET VerificationToken = '1234567890' where id = 15;

delete users where id=31;

select * from users;

ALTER TABLE users
ADD CONSTRAINT DF_users_lastlogin DEFAULT GETDATE() FOR lastlogin;

truncate table users;

drop table users;

CREATE TABLE items (
    id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(255) NOT NULL,
	catagory VARCHAR(255) NOT NULL,
    details VARCHAR(255) NOT NULL,
	user_id INT NOT NULL,
    imageurl1 VARCHAR(255),
	imageurl2 VARCHAR(255),
	imageurl3 VARCHAR(255),
	imageurl4 VARCHAR(255),
    createdat DATETIME DEFAULT GETDATE(),
    updatedat DATETIME DEFAULT GETDATE(),
    status INT DEFAULT 1 CHECK (status BETWEEN 0 AND 1),
	FOREIGN KEY (user_id) REFERENCES users(id)
);

ALTER TABLE items
ALTER COLUMN details TEXT;

drop table items;

select * from items;
select * from users;

UPDATE items SET imageurl1 = null where id = 5;

UPDATE items SET status = 1;

UPDATE users SET profilephoto = '/Uploads/ProfilePhotos/NullImages/status1.png' WHERE profilephoto IS NOT NULL;

CREATE TABLE requestitems (
    id INT PRIMARY KEY IDENTITY(1,1),
    sender_id INT NOT NULL,
	receiver_id INT NOT NULL,
	item_id INT NOT NULL,
    approved_status INT DEFAULT 2 CHECK (approved_status BETWEEN 0 AND 2), /*if 2 then pending,1 accepted,0 rejected*/
    createdat DATETIME DEFAULT GETDATE(),
    updatedat DATETIME DEFAULT GETDATE(),
	status INT DEFAULT 1 CHECK (status BETWEEN 0 AND 1),
	FOREIGN KEY (sender_id) REFERENCES users(id),
	FOREIGN KEY (receiver_id) REFERENCES users(id),
	FOREIGN KEY (item_id) REFERENCES items(id),
);

select * from requestitems;
update requestitems set approved_status = 2;

select * from requestitems where receiver_id = 1;

CREATE TABLE notifications (
    id INT PRIMARY KEY IDENTITY(1,1),
	user_id INT NOT NULL,
	details VARCHAR(255) NOT NULL,
	status INT DEFAULT 1 CHECK (status BETWEEN 0 AND 1),
    createdat DATETIME DEFAULT GETDATE(),
	FOREIGN KEY (user_id) REFERENCES users(id),
);

select * from notifications;
Update notifications SET status = 1;