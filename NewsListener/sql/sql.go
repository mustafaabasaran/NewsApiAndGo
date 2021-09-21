package sql

import (
	"NewsListener/shared"
	"context"
	"database/sql"
	_ "github.com/denisenkom/go-mssqldb"
	"log"
	"time"
)

func SqlOpen() *sql.DB {
	db, err := sql.Open("sqlserver", shared.Config.SQLURL)
	if err != nil {
		log.Fatal(err)
	}
	return db
}

func GetSqlContent(db *sql.DB) ([]string, []string, []string,[]string,[]string,[]time.Time, error) {
	var (
		SourceName  []string
		Author  	[]string
		Url     	[]string
		Title   	[]string
		Description []string
		InsertDate  []time.Time
		ctx         context.Context
	)
	ctx, cancel := context.WithTimeout(context.Background(), time.Minute)
	defer cancel()

	rows, err := db.QueryContext(ctx, "select SourceName, Author, Url, Title, Description,InsertDate from [dbo].[News]")
	if err != nil {
		log.Fatal(err)
	}
	defer rows.Close()
	for rows.Next() {
		var _sourceName string
		var _author string
		var _url string
		var _title string
		var _description string
		var _insertDate time.Time

		err := rows.Scan( &_sourceName, &_author, &_url, &_title, &_description, &_insertDate)
		if err != nil {
			return SourceName, Author, Url,Title, Description, InsertDate, err
		} else {
			SourceName  = append(SourceName, _sourceName)
			Author 	    = append(Author, _author)
			Url		    = append(Url, _url)
			Title 	    = append(Title, _title)
			Description = append(Description, _description)
			InsertDate  = append(InsertDate, _insertDate)
		}
	}
	return SourceName, Author, Url,Title, Description, InsertDate, nil
}

func InsertSqlContent(db *sql.DB, news *shared.AddNews) (int64, error) {
	stmt, err := db.Prepare("INSERT INTO dbo.News(SourceName, Author, Url, Title, Description) VALUES (@p1, @p2, @p3, @p4, @p5); select ID = convert(bigint, SCOPE_IDENTITY())")
	if err != nil {
		handleError(err, "Could not insert SqlDB")
		return 0, err
	}
	var ctx context.Context
	ctx, cancel := context.WithTimeout(context.Background(), time.Minute)
	defer cancel()

	defer stmt.Close()
	rows := stmt.QueryRowContext(ctx, news.SourceName, news.Author, news.Url, news.Title, news.Description)
	if rows.Err() != nil {
		return 0, err
	}
	var _id int64
	rows.Scan(&_id)
	return _id, nil
}

func handleError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}
}