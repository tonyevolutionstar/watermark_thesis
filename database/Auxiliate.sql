Use Watermark;

Select nome_ficheiro, inter_char, inter_point, date_time
from forense_analises inner join document 
	on document.id_document = forense_analises.id_doc 
where date_time = '20_3_2023_22_50_46'
order by inter_point

Select nome_ficheiro, date_time 
from document inner join watermark 
	on watermark.id_doc = document.id_document 
	order by date_time desc;

---
delete from forense_analises
where id_doc in (
select document.id_document from document left join watermark 
on document.id_document = watermark.id_doc left join forense_analises 
on forense_analises.id_doc = document.id_document where validacao IS NULL )

delete from position_char_file 
where id_doc in (
select position_char_file.id_doc from document left join watermark
	on document.id_document = watermark.id_doc left join position_char_file 
	on position_char_file.id_doc = document.id_document where validacao IS NULL)

delete from Document 
where id_document in (select document.id_document 
from document left join watermark on document.id_document = watermark.id_doc
left join forense_analises on forense_analises.id_doc = document.id_document where validacao IS NULL )

delete from barcode where id_barcode in (select id_barcode
from document left join watermark on document.id_document = watermark.id_doc
left join forense_analises on forense_analises.id_doc = document.id_document where validacao IS NULL) 


-- delete aux
DELETE FROM forense_analises
WHERE inter_point IN (
    SELECT inter_point
    FROM forense_analises
    GROUP BY id_doc, inter_point
	having  Count(inter_point) > 1
)
and id_doc = 1614377065