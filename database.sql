-- Holiday Maker: skapar databasen, tabellerna och testdata.
-- OBS: Skriptet raderar en befintlig databas med samma namn och börjar om.

DROP DATABASE IF EXISTS holiday_maker;
CREATE DATABASE holiday_maker;
USE holiday_maker;

-- ===== TABELLER =====

CREATE TABLE city (
    id   INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE accommodation (
    id                   INT AUTO_INCREMENT PRIMARY KEY,
    name                 VARCHAR(100) NOT NULL,
    city_id              INT          NOT NULL,
    has_pool             BOOLEAN      NOT NULL DEFAULT FALSE,
    has_restaurant       BOOLEAN      NOT NULL DEFAULT FALSE,
    distance_to_beach_m  INT          NOT NULL CHECK (distance_to_beach_m >= 0),
    distance_to_center_m INT          NOT NULL CHECK (distance_to_center_m >= 0),
    rating               TINYINT      NOT NULL CHECK (rating BETWEEN 1 AND 5),
    FOREIGN KEY (city_id) REFERENCES city (id),
    UNIQUE (name, city_id)
);

CREATE TABLE room (
    id               INT AUTO_INCREMENT PRIMARY KEY,
    accommodation_id INT           NOT NULL,
    room_number      VARCHAR(10)   NOT NULL,
    max_guests       TINYINT       NOT NULL CHECK (max_guests > 0),
    price_per_night  DECIMAL(10,2) NOT NULL CHECK (price_per_night > 0),
    FOREIGN KEY (accommodation_id) REFERENCES accommodation (id),
    UNIQUE (accommodation_id, room_number)
);

CREATE TABLE customer (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(50)  NOT NULL,
    last_name  VARCHAR(50)  NOT NULL,
    email      VARCHAR(100) NOT NULL UNIQUE,
    phone      VARCHAR(30)  NOT NULL
);

CREATE TABLE booking (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT      NOT NULL,
    room_id     INT      NOT NULL,
    check_in    DATE     NOT NULL,
    check_out   DATE     NOT NULL,
    guests      TINYINT  NOT NULL CHECK (guests > 0),
    created_at  DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (customer_id) REFERENCES customer (id),
    FOREIGN KEY (room_id) REFERENCES room (id),
    CHECK (check_out > check_in),
    INDEX idx_room_dates (room_id, check_in, check_out)
);

-- ===== TESTDATA =====

INSERT INTO city (id, name) VALUES
(1, 'Malmö'), (2, 'Ystad'), (3, 'Båstad');

INSERT INTO accommodation (id, name, city_id, has_pool, has_restaurant, distance_to_beach_m, distance_to_center_m, rating) VALUES
(1, 'Hotell Strandbris',  2, TRUE,  TRUE,  150, 1200, 5),
(2, 'Pensionat Lugnet',   2, FALSE, FALSE, 800,  300, 3),
(3, 'Malmö City Hotel',   1, FALSE, TRUE, 2500,  200, 4),
(4, 'Ribersborg Spa',     1, TRUE,  TRUE,  100, 3000, 4),
(5, 'Båstad Havshotell',  3, TRUE,  FALSE,  50,  900, 2);

INSERT INTO room (id, accommodation_id, room_number, max_guests, price_per_night) VALUES
(1, 1, '101', 2, 1450), (2, 1, '102', 2, 1450), (3, 1, '201', 4, 2600),
(4, 1, '202', 1,  950), (5, 1, '301', 6, 3900),
(6, 2, '1', 1, 550), (7, 2, '2', 2, 750), (8, 2, '3', 2, 800), (9, 2, '4', 4, 1200),
(10, 3, '11', 1, 890), (11, 3, '12', 2, 1190), (12, 3, '13', 2, 1290), (13, 3, '14', 4, 1990),
(14, 4, 'A1', 2, 1690), (15, 4, 'A2', 2, 1790), (16, 4, 'B1', 4, 2890),
(17, 4, 'B2', 1, 1100), (18, 4, 'C1', 6, 4200),
(19, 5, 'S1', 2, 1350), (20, 5, 'S2', 4, 2100);

INSERT INTO customer (id, first_name, last_name, email, phone) VALUES
(1, 'Anna',  'Lindqvist', 'anna.lindqvist@example.com', '070-1112233'),
(2, 'Erik',  'Johansson', 'erik.johansson@example.com', '070-2223344'),
(3, 'Sara',  'Nilsson',   'sara.nilsson@example.com',   '070-3334455'),
(4, 'Omar',  'Haddad',    'omar.haddad@example.com',    '070-4445566'),
(5, 'Lina',  'Berg',      'lina.berg@example.com',      '070-5556677');

INSERT INTO booking (customer_id, room_id, check_in, check_out, guests) VALUES
(1,  1, '2026-12-10', '2026-12-13', 2),
(2,  1, '2026-12-20', '2026-12-24', 2),
(3,  2, '2026-12-01', '2026-12-05', 1),
(4,  3, '2026-12-12', '2026-12-19', 4),
(5,  4, '2026-12-03', '2026-12-04', 1),
(1,  5, '2026-12-26', '2026-12-31', 5),
(2,  6, '2026-12-08', '2026-12-10', 1),
(3,  7, '2026-12-14', '2026-12-21', 2),
(4,  8, '2026-12-01', '2026-12-02', 2),
(5,  9, '2026-12-22', '2026-12-28', 3),
(1, 10, '2026-12-05', '2026-12-07', 1),
(2, 11, '2026-12-10', '2026-12-17', 2),
(3, 12, '2026-12-18', '2026-12-20', 1),
(4, 13, '2026-12-24', '2026-12-27', 4),
(5, 14, '2026-12-05', '2026-12-20', 2),
(1, 15, '2026-12-11', '2026-12-12', 2),
(2, 16, '2026-12-01', '2026-12-08', 4),
(3, 17, '2026-12-15', '2026-12-16', 1),
(4, 18, '2026-12-19', '2026-12-26', 6),
(5, 19, '2026-12-20', '2026-12-27', 2),
(1, 20, '2026-12-20', '2026-12-27', 4),
(2, 19, '2026-12-02', '2026-12-06', 2),
(3, 20, '2026-12-09', '2026-12-12', 3),
(4,  2, '2026-12-27', '2026-12-31', 2),
(5, 12, '2026-12-28', '2026-12-31', 2);