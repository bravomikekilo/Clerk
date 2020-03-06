import {HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel} from "@microsoft/signalr";

export const connection: HubConnection = new HubConnectionBuilder()
    .withUrl("/hub")
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Debug)
    .build();

export async function getConn(): Promise<HubConnection> {
    const currentState = connection.state;
    if (currentState == HubConnectionState.Disconnected) {
        await connection.start();
        console.log('signalR connected!');
        return connection;
    } else if(currentState == HubConnectionState.Connected) {
        return connection;
    } else if(currentState == HubConnectionState.Reconnecting) {
        return new Promise<HubConnection>((resolve, reject) => {
            connection.onreconnected(id => resolve(connection));
        })
    } else {
        await connection.start();
        return connection;
    }
}

