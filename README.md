# PushMessagesSender
The repository exists to demonstrate my corporate coding style. 
The code is incomplete and only contains a couple of methods, which are used to send, process and receive push messages.
The commentaries and documentation are in russian, because of the workplace policy.

The algorithm looks like this:

When a particular status in the system is changed, my API receives a JSON message. Based on the message, I check a few tables in the database, get some data, form a push message of specified format and send it to Google Firebase.
After sending, I check and log the response.