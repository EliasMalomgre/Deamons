package be.kdg.stemtest.model.repositories

import androidx.lifecycle.LiveData
import androidx.lifecycle.LiveDataReactiveStreams
import be.kdg.stemtest.model.Interactors.ConnectionDatabaseInteractor
import be.kdg.stemtest.model.Interactors.ConnectionRemoteInteractor
import be.kdg.stemtest.model.Interactors.OperationDatabaseInteractor
import be.kdg.stemtest.model.entity.GameSettings
import be.kdg.stemtest.model.entity.Identification
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class ConnectionRepository @Inject constructor(private val connectionRemoteInteractor: ConnectionRemoteInteractor,
                                               private val connectionDatabaseInteractor: ConnectionDatabaseInteractor,
                                               private val operationDatabaseInteractor: OperationDatabaseInteractor) {


    fun createIdentification(sessionCode: Int): LiveData<Identification> {

        val remote = connectionRemoteInteractor.addStudent(sessionCode)
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
            .retry(5)
            .onErrorReturn { Identification(sessionCode, -3) }
            .doOnSuccess{i ->
                pullSettings(i.sessionCode)
                connectionRemoteInteractor.getDefinitions(sessionCode)
                }

        return LiveDataReactiveStreams.fromPublisher(remote.toFlowable())
    }


    private fun pullSettings(sessionCode: Int) {
        connectionRemoteInteractor.getSettings(sessionCode)
            .doOnSuccess{ i ->
                if (i.gameType==1||i.gameType==2){
                    pullParties(sessionCode)
                }
            }
            .observeOn(AndroidSchedulers.mainThread())
            .subscribeOn(Schedulers.io())
            .subscribe()
    }

    private fun pullParties(sessionCode: Int) {
       connectionRemoteInteractor.getParties(sessionCode)
    }


    fun deleteAll() {
        operationDatabaseInteractor.deleteALL()
    }

    fun getGameSettings(): LiveData<GameSettings> {
        val database = connectionDatabaseInteractor.getGameSettings()
        return LiveDataReactiveStreams.fromPublisher(database)
    }

    fun getIdentification(): LiveData<Identification> {
        val database = connectionDatabaseInteractor.getIdentification()
       return LiveDataReactiveStreams.fromPublisher(database)
    }


}