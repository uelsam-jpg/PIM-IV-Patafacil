/**
 * AuthContext — gerencia a sessão do usuário no app.
 *
 * - Ao logar, salva o Token JWT e os dados do usuário no AsyncStorage
 *   (sincronização/durabilidade da sessão: o usuário continua logado ao reabrir o app).
 * - Expõe login/logout para as telas.
 */
import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { login as apiLogin, Usuario } from "../services/authService";

const STORAGE_TOKEN = "@patafacil/token";
const STORAGE_USUARIO = "@patafacil/usuario";

interface AuthContextData {
  token: string | null;
  usuario: Usuario | null;
  loading: boolean;
  login: (email: string, senha: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextData>({} as AuthContextData);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [usuario, setUsuario] = useState<Usuario | null>(null);
  const [loading, setLoading] = useState(true);

  // Restaura a sessão salva no dispositivo ao abrir o app
  useEffect(() => {
    (async () => {
      try {
        const [tokenSalvo, usuarioSalvo] = await Promise.all([
          AsyncStorage.getItem(STORAGE_TOKEN),
          AsyncStorage.getItem(STORAGE_USUARIO),
        ]);
        if (tokenSalvo && usuarioSalvo) {
          setToken(tokenSalvo);
          setUsuario(JSON.parse(usuarioSalvo));
        }
      } catch (e) {
        console.warn("Falha ao restaurar sessão", e);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const login = useCallback(async (email: string, senha: string) => {
    const resposta = await apiLogin(email, senha);
    setToken(resposta.token);
    setUsuario(resposta.usuario);
    await AsyncStorage.multiSet([
      [STORAGE_TOKEN, resposta.token],
      [STORAGE_USUARIO, JSON.stringify(resposta.usuario)],
    ]);
  }, []);

  const logout = useCallback(async () => {
    setToken(null);
    setUsuario(null);
    await AsyncStorage.multiRemove([STORAGE_TOKEN, STORAGE_USUARIO]);
  }, []);

  const value = useMemo(
    () => ({ token, usuario, loading, login, logout }),
    [token, usuario, loading, login, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextData {
  const contexto = useContext(AuthContext);
  if (!contexto) {
    throw new Error("useAuth deve ser usado dentro de um AuthProvider");
  }
  return contexto;
}
