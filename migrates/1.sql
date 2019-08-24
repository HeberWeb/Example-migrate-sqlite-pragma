/*Clique com botão esquerdo no arquivo e em "Copy output Directory" selecione Copy always
Este arquivo representa uma primeira versão de alteração da sua base de dados.
*/
create table users(
Id integer PRIMARY KEY,
Name text not null,
Login text not null,
Password text not null
);

alter table clients
add CNPJ text not null default 0;

PRAGMA user_version = 1;
