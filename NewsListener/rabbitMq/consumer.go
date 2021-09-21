package rabbitMq

import (
	"NewsListener/shared"
	newsSql "NewsListener/sql"
	"database/sql"
	"encoding/json"
	"fmt"
	"github.com/streadway/amqp"
	"log"
	"os"
	"strings"
)

func Consumer(db *sql.DB) {
	conn, err := amqp.Dial(shared.Config.AMQPURL)
	handleError(err, "Can't connect to AMQP")
	defer conn.Close()
	amqpChannel, err := conn.Channel()
	handleError(err, "Can't create a amqpChannel")

	//defer amqpChannel.Close()

	q, err := amqpChannel.QueueDeclare(
		shared.Config.ROUTING_KEY, // name
		true,         // durable
		false,        // delete when unused
		false,        // exclusive
		false,        // no-wait
		nil,          // arguments
	)
	handleError(err, "Failed to declare an exchange")

	err = amqpChannel.Qos(
		1,     // prefetch count
		0,     // prefetch size
		false, // global
	)
	handleError(err, "Failed to set QoS")

	messageChannel, err := amqpChannel.Consume(
		q.Name, // queue
		"",     // consumer
		false,  // auto-ack
		false,  // exclusive
		false,  // no-local
		false,  // no-wait
		nil,    // args
	)
	handleError(err, "Failed to register a consumer")

	stopChan := make(chan bool)

	go func() {
		log.Printf("Consumer ready, PID: %d", os.Getpid())
		for d := range messageChannel {
			fmt.Println(strings.Repeat("-", 100))
			log.Printf("Received a message: %s", d.Body)

			addNews := &shared.AddNews{}

			err := json.Unmarshal(d.Body, addNews)
			if err != nil {
				log.Printf("Error decoding JSON: %s", err)
			}

			fmt.Println(strings.Repeat("-", 100))
			fmt.Printf("New :%s - Url: %s\n", addNews.Description, addNews.Url)

			log.Printf("Subject:  %s. Author : %s", addNews.Title, addNews.Author)
			res, err2 := newsSql.InsertSqlContent(db, addNews)
			handleError(err2, "Could not Insert Product to Sql")
			log.Printf("Inserted News ID : %d", res)

			if err := d.Ack(false); err != nil {
				log.Printf("Error acknowledging message : %s", err)
			} else {
				log.Printf("Acknowledged message")
			}
		}
	}()

	// Stop for program termination
	<-stopChan
}

func handleError(err error, msg string) {
	if err != nil {
		log.Fatalf("%s: %s", msg, err)
	}
}