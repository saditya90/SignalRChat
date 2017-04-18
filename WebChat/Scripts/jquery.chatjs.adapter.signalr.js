
var SignalRServerAdapter = (function () {
    function SignalRServerAdapter(chatHubServer) {
        this.hubServer = chatHubServer;
    }
    // sends a message to a room, conversation or user
    SignalRServerAdapter.prototype.sendMessage = function (roomId, conversationId, otherUserId, messageText, clientGuid, done) {
        this.hubServer.sendMessage(roomId, conversationId, otherUserId, messageText, clientGuid).done(function () {
            done();
        });
    };

    // sends a typing signal to a room, conversation or user
    SignalRServerAdapter.prototype.sendTypingSignal = function (roomId, conversationId, userToId, done) {
        this.hubServer.sendTypingSignal(roomId, conversationId, userToId).done(function () {
            done();
        });
    };

    // gets the message history from a room, conversation or user
    SignalRServerAdapter.prototype.getMessageHistory = function (roomId, conversationId, otherUserId, done) {
        this.hubServer.getMessageHistory(roomId, conversationId, otherUserId).done(function (messageHistory) {
            done(JSON.parse(messageHistory));
        });
    };

    // gets the given user info
    SignalRServerAdapter.prototype.getUserInfo = function (userId, done) {
        this.hubServer.getUserInfo(userId).done(function (userInfo) {
            var user = JSON.parse(userInfo);
            done(user);
        });
    };

    // gets the user list in a room or conversation
    SignalRServerAdapter.prototype.getUserList = function (roomId, conversationId, done) {
        this.hubServer.getUserList(roomId, conversationId).done(function (userList) {
            if (userList !== "") {
                var objects = JSON.parse(userList);
                done(objects);
            }
            else { done([]); }
        });
    };

    // gets the rooms list
    SignalRServerAdapter.prototype.getRoomsList = function (done) {
        this.hubServer.getRoomsList().done(function (roomsList) {
            done(roomsList);
        });
    };

    // enters the given room
    SignalRServerAdapter.prototype.enterRoom = function (roomId, done) {
        this.hubServer.enterRoom(roomId).done(function () {
            done();
        });
    };

    // leaves the given room
    SignalRServerAdapter.prototype.leaveRoom = function (roomId, done) {
        this.hubServer.leaveRoom(roomId).done(function () {
            done();
        });
    };
    return SignalRServerAdapter;
})();

var SignalRClientAdapter = (function () {
    function SignalRClientAdapter(chatHubClient) {
        var _this = this;
        this.messagesChangedHandlers = [];
        this.typingSignalReceivedHandlers = [];
        this.userListChangedHandlers = [];
        this.roomListChangedHandlers = [];
        this.hubClient = chatHubClient;

        // called by the server when a new message arrives
        this.hubClient.sendMessage = function (message) {
            var msg = JSON.parse(message);
            if ($("div.chat-window[data-winid='div_" + msg.UserFromId + "']").length == 0) {
                $("div.user-list-item[data-val-id='" + msg.UserFromId + "']").trigger("click");
                window.setTimeout(function () {
                    if ($("div.chat-window[data-winid='div_" + msg.UserFromId + "'] div.messages-wrapper div.chat-message").length == 0) {
                        _this.triggerMessagesChanged(msg);
                    }
                }, 1000);
            }
            else { _this.triggerMessagesChanged(msg); }
        };

        this.hubClient.sendTypingSignal = function (typingSignal) {
            _this.triggerTypingSignalReceived(JSON.parse(typingSignal));
        };

        this.hubClient.userListChanged = function (userListChangedInfo) { 
            _this.triggerUserListChanged(JSON.parse(userListChangedInfo));
        };

        this.hubClient.roomListChanged = function (roomListChangedInfo) {
            _this.triggerRoomListChanged(roomListChangedInfo);
        };
    }
    // adds a handler to the messagesChanged event
    SignalRClientAdapter.prototype.onMessagesChanged = function (handler) {
        this.messagesChangedHandlers.push(handler);
    };

    // adds a handler to the typingSignalReceived event
    SignalRClientAdapter.prototype.onTypingSignalReceived = function (handler) { 
        this.typingSignalReceivedHandlers.push(handler);
    };

    // adds a handler to the userListChanged event
    SignalRClientAdapter.prototype.onUserListChanged = function (handler) {
        this.userListChangedHandlers.push(handler);
    };

    // adds a handler to the roomListChanged
    SignalRClientAdapter.prototype.onRoomListChanged = function (handler) {
        this.roomListChangedHandlers.push(handler);
    };

    SignalRClientAdapter.prototype.triggerMessagesChanged = function (message) {
        for (var i = 0; i < this.messagesChangedHandlers.length; i++)
            this.messagesChangedHandlers[i](message);
    };

    SignalRClientAdapter.prototype.triggerTypingSignalReceived = function (typingSignal) {
        for (var i = 0; i < this.typingSignalReceivedHandlers.length; i++)
            this.typingSignalReceivedHandlers[i](typingSignal);
    };

    SignalRClientAdapter.prototype.triggerUserListChanged = function (userListChangedInfo) {
        for (var i = 0; i < this.userListChangedHandlers.length; i++)
            this.userListChangedHandlers[i](userListChangedInfo);
    };

    SignalRClientAdapter.prototype.triggerRoomListChanged = function (roomListChangedInfo) {
        for (var i = 0; i < this.roomListChangedHandlers.length; i++)
            this.roomListChangedHandlers[i](roomListChangedInfo);
    };
    return SignalRClientAdapter;
})();

var SignalRAdapterOptions = (function () {
    function SignalRAdapterOptions() {
    }
    return SignalRAdapterOptions;
})();

var SignalRAdapter = (function () {
    function SignalRAdapter(options) {
        var defaultOptions = new SignalRAdapterOptions();
        defaultOptions.chatHubName = "chatHub";
        this.options = $.extend({}, defaultOptions, options);
    }
    SignalRAdapter.prototype.init = function (done) {
        this.hub = $.connection[this.options.chatHubName];
        this.client = new SignalRClientAdapter(this.hub.client);
        this.server = new SignalRServerAdapter(this.hub.server);

        if (!window.chatJsHubReady)
            window.chatJsHubReady = $.connection.hub.start();
        window.chatJsHubReady.done(function () {
            // function passed by ChatJS to the adapter to be called when the adapter initialization is completed 
            done();
        });
    };
    return SignalRAdapter;
})();
//# sourceMappingURL=jquery.chatjs.adapter.signalr.js.map
