def message = "";

pipeline {
	agent any
	
	environment {
		BuildConfiguration="Debug"
        ImageNameGameProviders="registry.gitlab.com/rayprosoft/turnkey-gaming-system/game-gameproviders"
		ImageTagUrlGameProviders = "https://gitlab.com/api/v4/projects/20478010/registry/repositories/2395683/tags"
        ImageGameProviders = "game-gameproviders"
		ImageNameKickplayerReceiverServer="registry.gitlab.com/rayprosoft/turnkey-gaming-system/game-kickplayer-receiverserver"
		ImageTagUrlKickplayerReceiverServer = "https://gitlab.com/api/v4/projects/20478010/registry/repositories/2355332/tags"
        ImageKickplayerReceiverServer = "game-kickplayer-receiverserver"
		ImageNameGamerecordReceiverServer ="registry.gitlab.com/rayprosoft/turnkey-gaming-system/game-gamerecord-receiverserver"
		ImageTagUrlGamerecordReceiverServer = "https://gitlab.com/api/v4/projects/20478010/registry/repositories/2355451/tags"
        ImageGamerecordReceiverServer = "game-gamerecord-receiverserver"
	}

	stages {
		stage('Prepare') {
            steps {
                sh "echo 'Branch: $GIT_BRANCH'";
                script {
                    switch(env.GIT_BRANCH)
                    {
                        case ['origin/main','main']:
                            env.kube_ns = "prod";
                            env.k8s_config = 'stag-k8s-config';
                            break;
                        case ['origin/release','release']:
                            env.kube_ns = "staging";
                            env.k8s_config = 'stag-k8s-config';
                            break;
                        default:
                            env.kube_ns = "dev";
                            env.k8s_config = 'k8s-kube-config';
                            break;
                    }
                }
                configFileProvider([
                    configFile(fileId: 'tgs.' + env.kube_ns , variable: 'tgsconfig')]) {
                        script {
                            def envItems = readProperties file: "$tgsconfig";
                            envItems.each { key, value -> env."tgs_${key}" = value;
                        }
                        env.TAG = sh(script: "echo `date +%Y%m%d%H%M`", returnStdout: true).trim() + '-' + env.kube_ns;
                        message += "\nBranch: ${env.GIT_BRANCH}" 
                        message += "\nTag: ${env.TAG}"
                        message += "\nEnv: ${env.kube_ns}"
                        env.commit_url = "${env.GIT_URL}".replace("git@gitlab.com:","https://gitlab.com/")
                            .replace(".git","/-/commit/") + "${env.GIT_COMMIT}"
                        message += "\n Commit: ${env.commit_url}"
                    }
                }
                sh "echo tag: $TAG"
            }
        }

		stage('Build') {
			steps {
				sh 'dotnet build -c $BuildConfiguration'
			}
		}

		stage('Test') {
			steps {
				sh 'dotnet test -c $BuildConfiguration --logger trx --no-build'
			}
		}

        stage('Push GameProviders Clinet packages') {
			steps {
				sh 'dotnet pack -c $BuildConfiguration "TGS.GameProviders.Client/TGS.GameProviders.Client.csproj" --no-build'
				sh 'dotnet nuget push "TGS.GameProviders.Client/bin/$BuildConfiguration/*.nupkg" --source "raypro-baget" -k admin --skip-duplicate'
			}
		}

        stage('Build Game GameProviders Docker Image') {
			steps {
				sh 'dotnet build "TGS.GameProviders/TGS.GameProviders.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageNameGameProviders:${TAG}" --no-restore '
			}
		}

		stage('Build Game Kickplayer Receiver Server Docker Image') {
			steps {
				sh 'dotnet build "TGS.GameProviders.KickPlayer.ReceiverServer/TGS.GameProviders.KickPlayer.ReceiverServer.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageNameKickplayerReceiverServer:${TAG}" --no-restore '
			}
		}

        stage('Build Game GameRecord Receiver Server Docker Image') {
			steps {
                sh 'dotnet build "TGS.GameProviders.GameRecord.ReceiverServer/TGS.GameProviders.GameRecord.ReceiverServer.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageNameGamerecordReceiverServer:${TAG}" --no-restore '
			}
		}

		stage('Push Game GameProviders Docker Image') {
			steps {
				sh 'sudo docker push $ImageNameGameProviders:${TAG}'
			}
		}

		stage('Push Game Kickplayer Receiver Server Docker Image') {
			steps {
				sh 'sudo docker push $ImageNameKickplayerReceiverServer:${TAG}'
			}
		}

        stage('Push Game GameRecord Receiver Server Docker Image') {
			steps {
                sh 'sudo docker push $ImageNameGamerecordReceiverServer:${TAG}'
			}
		}

		stage('Deploy') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }

            steps {
                script {
                    input message: 'Ready to go?', ok: 'yes';
                }
                
                sh "echo start deploy"
            }
        }

		stage('Deploy Game GameProviders to k8s') {
			steps {
				withKubeConfig([credentialsId: env.k8s_config]) {
                    withCredentials([string(credentialsId: "tgs-${kube_ns}-connectionstring", variable: "tgs_tgs")]) {
                        sh '''
                            helm template deployment/k8s/game-gameproviders \
                                --set image.tag=${TAG} \
                                --set namespace=${kube_ns} \
                                --set autoscaling.enabled=${tgs_autoscaling} \
                                --set connectionStrings.tgs="$tgs_tgs" \
                                --set connectionStrings.redis="$tgs_redis" \
                                --set connectionStrings.mongo="$tgs_mongo" \
                                --set nats.urls="$tgs_nats" \
                        '''
                        sh '''
                            helm template deployment/k8s/game-gameproviders \
                                --set image.tag=${TAG} \
                                --set namespace=${kube_ns} \
                                --set autoscaling.enabled=${tgs_autoscaling} \
                                --set connectionStrings.tgs="$tgs_tgs" \
                                --set connectionStrings.redis="$tgs_redis" \
                                --set connectionStrings.mongo="$tgs_mongo" \
                                --set nats.urls="$tgs_nats" \
                                | kubectl -n ${kube_ns} apply -f -
                        '''
                    }
				}
			}
		}

		stage('Deploy Game Kickplayer Receiver Server to k8s') {
			steps {
				withKubeConfig([credentialsId: env.k8s_config]) {
                    withCredentials([string(credentialsId: "tgs-${kube_ns}-connectionstring", variable: "tgs_tgs")]) {
                        sh '''
                            helm template deployment/k8s/game-kickplayer-receiverserver \
                                --set image.tag=${TAG} \
                                --set namespace=${kube_ns} \
                                --set autoscaling.enabled=${tgs_autoscaling} \
                                --set connectionStrings.tgs="$tgs_tgs" \
                                --set connectionStrings.redis="$tgs_redis" \
                                --set connectionStrings.mongo="$tgs_mongo" \
                                --set nats.urls="$tgs_nats" \
                                --set rabbit.url="$tgs_rabbit_url" \
                                --set rabbit.user="$tgs_rabbit_user" \
                                --set rabbit.password="$tgs_rabbit_pass" \
                                --set rabbit.vhost="$tgs_rabbit_vhost"
                        '''
                        sh '''
                            helm template deployment/k8s/game-kickplayer-receiverserver \
                                --set image.tag=${TAG} \
                                --set namespace=${kube_ns} \
                                --set autoscaling.enabled=${tgs_autoscaling} \
                                --set connectionStrings.tgs="$tgs_tgs" \
                                --set connectionStrings.redis="$tgs_redis" \
                                --set connectionStrings.mongo="$tgs_mongo" \
                                --set nats.urls="$tgs_nats" \
                                --set rabbit.url="$tgs_rabbit_url" \
                                --set rabbit.user="$tgs_rabbit_user" \
                                --set rabbit.password="$tgs_rabbit_pass" \
                                --set rabbit.vhost="$tgs_rabbit_vhost" \
                                | kubectl -n ${kube_ns} apply -f -
                        '''
                    }
				}
			}
		}
        
        stage('Deploy Game GameRecord Receiver Server to k8s') {
			steps {
				withKubeConfig([credentialsId: env.k8s_config]) {
                    withCredentials([string(credentialsId: "tgs-${kube_ns}-connectionstring", variable: "tgs_tgs")]) {
                        sh '''
                            helm template deployment/k8s/game-gamerecord-receiverserver \
                                --set image.tag=${TAG} \
                                --set namespace=${kube_ns} \
                                --set autoscaling.enabled=${tgs_autoscaling} \
                                --set connectionStrings.tgs="$tgs_tgs" \
                                --set connectionStrings.redis="$tgs_redis" \
                                --set connectionStrings.mongo="$tgs_mongo" \
                                --set nats.urls="$tgs_nats" \
                                --set rabbit.url="$tgs_rabbit_url" \
                                --set rabbit.user="$tgs_rabbit_user" \
                                --set rabbit.password="$tgs_rabbit_pass" \
                                --set rabbit.vhost="$tgs_rabbit_vhost"
                        '''
                        sh '''
                            helm template deployment/k8s/game-gamerecord-receiverserver \
                                --set image.tag=${TAG} \
                                --set namespace=${kube_ns} \
                                --set autoscaling.enabled=${tgs_autoscaling} \
                                --set connectionStrings.tgs="$tgs_tgs" \
                                --set connectionStrings.redis="$tgs_redis" \
                                --set connectionStrings.mongo="$tgs_mongo" \
                                --set nats.urls="$tgs_nats" \
                                --set rabbit.url="$tgs_rabbit_url" \
                                --set rabbit.user="$tgs_rabbit_user" \
                                --set rabbit.password="$tgs_rabbit_pass" \
                                --set rabbit.vhost="$tgs_rabbit_vhost" \
                                | kubectl -n ${kube_ns} apply -f -
                        '''
                    }
				}
			}
		}

		stage('Add Tag on Prod') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }			
            steps {	
				sshagent(['ssh-jenkins']) {
                    sh "git tag -a ${TAG} -m ${TAG}"
                    sh "git push ${GIT_URL} ${TAG} HEAD:${GIT_BRANCH}"
                }
            }
        }

        stage('Remove Game GameProviders Image Tags') {					
            steps {	
                withCredentials([string(credentialsId: "GitLabToken", variable: "GitLabToken")]) {					
                    sh "curl --request DELETE --data \'name_regex_delete=.*\' --data \'keep_n=10\' --header \'PRIVATE-TOKEN: ${GitLabToken}\' ${ImageTagUrlGameProviders}"
                }
            }
        }

        stage('Remove Game Kickplayer Receiver Server Image Tags') {					
            steps {	
                withCredentials([string(credentialsId: "GitLabToken", variable: "GitLabToken")]) {					
                    sh "curl --request DELETE --data \'name_regex_delete=.*\' --data \'keep_n=10\' --header \'PRIVATE-TOKEN: ${GitLabToken}\' ${ImageTagUrlKickplayerReceiverServer}"
                }
            }
        }

        stage('Remove Game GameRecord Receiver Server Image Tags') {					
            steps {	
                withCredentials([string(credentialsId: "GitLabToken", variable: "GitLabToken")]) {					
                    sh "curl --request DELETE --data \'name_regex_delete=.*\' --data \'keep_n=10\' --header \'PRIVATE-TOKEN: ${GitLabToken}\' ${ImageTagUrlGamerecordReceiverServer}"
                }
            }
        }
	}

	post {
		always {
			step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
		}
		success {
             sendTelegram("Pipeline: ${currentBuild.fullDisplayName} OK! ${message} \n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
        failure {
            sendTelegram("<b>Failed Pipeline:</b> ${currentBuild.fullDisplayName} Something is wrong with ${env.BUILD_URL}/console\n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
	}
}


def sendTelegram(message) {
    def encodedMessage = URLEncoder.encode(message, "UTF-8")

    withCredentials([string(credentialsId: "telegramToken", variable: "TOKEN"), string(credentialsId: "telegramChatId", variable: "CHAT_ID")]) {

        response = httpRequest (consoleLogResponseBody: true,
                contentType: 'APPLICATION_JSON',
                httpMode: 'GET',
                url: "https://api.telegram.org/bot${TOKEN}/sendMessage?text=${encodedMessage}&chat_id=${CHAT_ID}",
                validResponseCodes: '200')
        return response
    }
}
