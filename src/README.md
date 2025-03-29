https://medium.com/@masreis/text-extraction-and-ocr-with-apache-tika-302464895e5f

What's going on:

* Postgres with vector extension
* Custom built docker image - tika with english tesseract 
* 

TODO:

* Create migration for TikaCache table
* Document upload page (name & file inputs)
* Save document
* Document list page
* Send document to Tika
* Breakup into pages (best-guess for docx) (should docx's be converted to pdfs first before sent to tika? how?)
* Generate embeddings for pages and store search chunks
* Search page (execute search and show results, jump to page).