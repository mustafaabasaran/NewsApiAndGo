package shared

var QueueName = "newsapi_bus"

type Configuration struct {
	AMQPURL     string
	SQLURL      string
	BROKER_NAME string
	ROUTING_KEY string
}

var Config = Configuration{
	AMQPURL:     "amqp://guest:guest@localhost:5672/",
	SQLURL:      "sqlserver://sa:*******@localhost?database=NewsDb&connection+timeout=30",
	BROKER_NAME: "newsapi_bus",
	ROUTING_KEY: "NewsModel",
}

type AddNews struct {
	SourceName  string
	Author      string
	Url         string
	Title       string
	Description string
}
