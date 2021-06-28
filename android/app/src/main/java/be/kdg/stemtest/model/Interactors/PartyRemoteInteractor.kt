package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.entity.Party
import be.kdg.stemtest.model.rest.RemoteDataSource
import io.reactivex.Completable
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class PartyRemoteInteractor @Inject constructor(private val remoteDataSource: RemoteDataSource,
                                                private val connectionDatabaseInteractor: ConnectionDatabaseInteractor
) {
    fun chosenParty(chosenParty: Party): Completable {
        val identification = connectionDatabaseInteractor
            .getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()

        return remoteDataSource.getStemTestService()
            .selectParty(identification.sessionCode, identification.studentId, chosenParty.name)
    }

}