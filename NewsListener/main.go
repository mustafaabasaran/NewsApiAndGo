package main

import (
	"NewsListener/rabbitMq"
	"NewsListener/sql"
	"fmt"
	"strings"
)

func main()  {
	var db = sql.SqlOpen()
	defer db.Close()

	//GetSqlContent
	sourceName, author, url, title, description, insertDate, err := sql.GetSqlContent(db)
	if err != nil {
		fmt.Println("(sqltest) Error getting content: " + err.Error())
	}
	fmt.Println(strings.Repeat("-", 100))
	// Now read the contents
	for i := range sourceName {
		fmt.Println("SourceName : " +  sourceName[i] + ", Author : " + author[i] + ", Url :" + url[i] +  ", Title : " + title[i] + ", Description: " + description[i] + ", InsertDate : " + insertDate[i].String())
	}

	//RabbitMQ Consumer
	rabbitMq.Consumer(db)
}
