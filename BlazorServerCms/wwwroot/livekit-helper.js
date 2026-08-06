// wwwroot/livekit-helper.js
import { Room, RoomEvent } from 'https://cdn.jsdelivr.net/npm/livekit-client@1.15.5/+esm';

let roomActive = null;

// Colocamos as funções no escopo global para o Blazor achar sem erro
window.livekitHelper = {
    
    conectarNaLive: async function (urlServidor, tokenAcesso, elementoVideoId) {
        roomActive = new Room({
            adaptiveStream: true,
            dynacast: true,
        });

        roomActive.on(RoomEvent.TrackSubscribed, (track, publication, participant) => {
            if (track.kind === 'video' || track.kind === 'audio') {
                const containerVideo = document.getElementById(elementoVideoId);
                if (containerVideo) {
                    const elementoMidia = track.attach();
                    elementoMidia.style.width = "100%";
                    elementoMidia.style.borderRadius = "8px";
                    containerVideo.appendChild(elementoMidia);
                }
            }
        });

        // 2. NOVO EVENTO: Quando o streamer sai e o vídeo é interrompido!
    roomActive.on(RoomEvent.TrackUnsubscribed, (track, publication, participant) => {
        const containerVideo = document.getElementById(elementoVideoId);
        if (containerVideo) {
            // Remove o player antigo da tela
            track.detach(); 
            
            // Injeta uma mensagem amigável para o espectador saber o que houve
            containerVideo.innerHTML = `
                <div style="display: flex; flex-direction: column; align-items: center; justify-content: center; height: 100%; color: #fff; font-family: sans-serif; padding: 20px;">
                    <span style="font-size: 40px;">🛑</span>
                    <h4 style="margin: 10px 0 5px 0;">A transmissão foi encerrada</h4>
                    <p style="color: #aaa; margin: 0; font-size: 14px;">O streamer saiu da sala ou perdeu a conexão.</p>
                </div>
            `;
        }
    });

        await roomActive.connect(urlServidor, tokenAcesso);
    },

iniciarTransmissaoPeloCelular: async function (urlServidor, tokenAcesso, elementoVideoId) {
    // 1. Instancia a sala
    roomActive = new Room({
        adaptiveStream: true,
        dynacast: true
    });

    // 2. Conecta ao LiveKit
    await roomActive.connect(urlServidor, tokenAcesso);

    if (!roomActive.localParticipant) {
        console.error("Participante local não encontrado.");
        return;
    }

    // 3. Ativa a câmera e o microfone do aparelho
    await roomActive.localParticipant.enableCameraAndMicrophone();

    // 4. Aguarda a publicação da track de forma segura
    let tentativas = 0;
    while (tentativas < 30) {
        const tracks = roomActive.localParticipant.videoTracks; // Pegar direto pelo mapeador interno do LiveKit
        if (tracks && tracks.size > 0) {
            break;
        }
        await new Promise(resolve => setTimeout(resolve, 150));
        tentativas++;
    }

    // 5. Captura o elemento HTML da div do Blazor
    const containerVideo = document.getElementById(elementoVideoId);
    if (!containerVideo) return;

    // 6. Resgata a track de vídeo cadastrada sem usar .values()
    const tracksDeVideo = Array.from(roomActive.localParticipant.videoTracks.values());
    const primeiraTrack = tracksDeVideo[0]?.track;

    if (primeiraTrack) {
        // Renderiza o vídeo na tela
        const elementoMidia = primeiraTrack.attach();
        elementoMidia.style.width = "100%";
        elementoMidia.style.height = "100%";
        elementoMidia.style.objectFit = "cover";
        elementoMidia.style.transform = "scaleX(-1)"; // Espelha a câmera frontal
        elementoMidia.style.borderRadius = "12px";
        
        containerVideo.innerHTML = ""; // Limpa carregamento anterior
        containerVideo.appendChild(elementoMidia);
    } else {
        console.error("A câmera está ativa, mas a faixa de vídeo demorou para responder.");
    }
},

    desconectarDaLive: async function () {
        if (roomActive) {
            await roomActive.disconnect();
            roomActive = null;
        }
    }
};