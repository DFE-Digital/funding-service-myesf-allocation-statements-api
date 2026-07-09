function setAllocationStatementAsReadById(id, principle) {
    var collection = getContext().getCollection();

    var query = {
        query: "SELECT * from c WHERE c.id = @ID",
        parameters: [
            { name: "@ID", value: id }
        ]
    };

    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        query,

        function (err, documents) {
            if (err) throw err;

            if (documents.length > 0) {
                //Allocations found. Set has been read property.
                documents.forEach(function (doc) {
                    doc.history.push({
                        "Action": "ViewedInDetail",
                        "User": {
                            "Principle": principle
                        },
                        "ActionDateTimeUtc": new Date().toISOString(),
                        "Message": null
                    });

                    var accept = collection.replaceDocument(doc._self, doc,
                        function (err) {
                            if (err) throw 'Unable to update allocation, abort ';
                        });

                    if (!accept) throw 'Unable to update allocation, abort';
                }, this);
            }
            else {
                // Else a document with the given id does not exist..
                throw new Error('Allocation not found.');
            }
            var response = getContext().getResponse();
            response.setBody(true);
        });

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}