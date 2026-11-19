-- Consultas para conferir no Oracle (SQL Developer) o que a API gravou.
-- As tabelas abaixo foram criadas pelas Migrations do EF Core, não à mão.

-- 1) Tabelas criadas pelas Migrations
SELECT table_name FROM user_tables
 WHERE table_name LIKE 'VS_%' OR table_name = '__EFMigrationsHistory'
 ORDER BY table_name;

-- 2) Migrations aplicadas (o nome da tabela é sensível a maiúsculas, por isso as aspas)
SELECT "MigrationId", "ProductVersion" FROM "__EFMigrationsHistory" ORDER BY 1;

-- 3) Colunas e tipos gerados
SELECT table_name, column_name, data_type, data_length, nullable
  FROM user_tab_columns
 WHERE table_name IN ('VS_UNIDADE_SAUDE', 'VS_TRANSPORTE')
 ORDER BY table_name, column_id;

-- 4) Chaves primárias, estrangeira e índice único (relacionamento 1:N)
SELECT constraint_name, constraint_type, table_name, r_constraint_name
  FROM user_constraints
 WHERE table_name IN ('VS_UNIDADE_SAUDE', 'VS_TRANSPORTE')
   AND constraint_type IN ('P', 'R')
 ORDER BY table_name, constraint_type;

SELECT index_name, uniqueness, table_name
  FROM user_indexes
 WHERE table_name = 'VS_TRANSPORTE';

-- 5) Dados gravados pela API
SELECT * FROM VS_UNIDADE_SAUDE ORDER BY ID;
SELECT * FROM VS_TRANSPORTE ORDER BY ID;

-- 6) Relacionamento 1:N na prática: cada transporte com a sua unidade de destino
SELECT t.ID, t.CODIGO, t.STATUS, t.CRIADO_EM, t.RECEBIDO_EM, u.NOME AS UNIDADE_DESTINO
  FROM VS_TRANSPORTE t
  JOIN VS_UNIDADE_SAUDE u ON u.ID = t.UNIDADE_DESTINO_ID
 ORDER BY t.ID;

-- 7) Quantos transportes cada unidade recebe
SELECT u.ID, u.NOME, COUNT(t.ID) AS QTD_TRANSPORTES
  FROM VS_UNIDADE_SAUDE u
  LEFT JOIN VS_TRANSPORTE t ON t.UNIDADE_DESTINO_ID = u.ID
 GROUP BY u.ID, u.NOME
 ORDER BY u.ID;
