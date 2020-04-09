export interface CreateGameCommand {
    name: string;
    password: string;
    maxPlayers: number;
    adminName: string;
    adminBuyIn: number;
    smallBlind: number;
    bigBlind: number;
}