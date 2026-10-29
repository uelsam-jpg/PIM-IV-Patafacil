import { NavigatorScreenParams } from "@react-navigation/native";

/** Pilha de telas antes de logar */
export type AuthStackParamList = {
  Login: undefined;
  Register: undefined;
};

/** Abas do app depois de logar */
export type MainTabsParamList = {
  Home: undefined;
  Pets: undefined;
  Agendar: undefined;
  MeusAgendamentos: undefined;
  Perfil: undefined;
  /** Aba oculta (não aparece na barra) — acessada pela tela de Pets */
  AddPet: undefined;
};

/** Pilha principal do app */
export type RootStackParamList = {
  Auth: NavigatorScreenParams<AuthStackParamList> | undefined;
  Main: NavigatorScreenParams<MainTabsParamList> | undefined;
};
