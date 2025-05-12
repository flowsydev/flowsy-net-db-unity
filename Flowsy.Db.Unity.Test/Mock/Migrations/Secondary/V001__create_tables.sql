create table customer
(
    customer_id int auto_increment primary key,
    name varchar(255) not null unique,
    email varchar(255) not null unique,
    created_at timestamp default current_timestamp(),
    updated_at timestamp default current_timestamp()
);